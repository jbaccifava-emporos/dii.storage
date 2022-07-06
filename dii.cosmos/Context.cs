﻿using dii.cosmos.Exceptions;
using dii.cosmos.Models;
using dii.cosmos.Models.Interfaces;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace dii.cosmos
{
    public class Context
	{
		#region Private Fields
		private static Context _instance;
		private static object _instanceLock = new object();
		#endregion Private Fields

		#region Public Fields
		public readonly INoSqlDatabaseConfig Config;
		public readonly CosmosClient Client;
		#endregion Public Fields

		#region Public Properties
		public Database Db { get; private set; }
		public DatabaseProperties DbProperties { get; private set; }
		public int? DbThroughput { get; private set; }
		public bool DatabaseCreatedThisContext { get; private set; }
		public Dictionary<Type, TableMetaData> TableMappings { get; private set; }
		#endregion Public Properties

		#region Constructors
		private Context(INoSqlDatabaseConfig config)
		{
			Config = config;

			CosmosClientOptions cosmosClientOptions = null;
			if (config.AllowBulkExecution.HasValue)
            {
				cosmosClientOptions = new CosmosClientOptions
				{
					AllowBulkExecution = config.AllowBulkExecution.Value
				};
            }

			Client = new CosmosClient(Config.Uri, Config.Key, cosmosClientOptions);
		}
		#endregion Constructors

		#region Public Methods
		public static Context Init(INoSqlDatabaseConfig config)
		{
			if (_instance == null)
			{
				lock (_instanceLock)
				{
					if (_instance == null)
					{
						_instance = new Context(config);
					}
				}
			}

			return _instance;
		}

		public static Context Get()
		{
			if (_instance == null)
			{
				throw new DiiNotInitializedException(nameof(Context));
			}

			return _instance;
		}

		/// <summary>
		/// Checks if the database exists and creates if it does not.
		/// </summary>
		/// <returns>Should always return true or throw an exception.</returns>
		public async Task<bool> DoesDatabaseExistAsync()
		{
			if (Config.AutoCreate)
			{
				var throughputProperties = Config.AutoScaling ?
							ThroughputProperties.CreateAutoscaleThroughput(Config.MaxRUPerSecond)
							: ThroughputProperties.CreateManualThroughput(Config.MaxRUPerSecond);

				var response = await Client.CreateDatabaseIfNotExistsAsync(Config.DatabaseId, throughputProperties).ConfigureAwait(false);

				Db = response.Database;
				DbProperties = response.Resource;

				//Skip Throughput check if DB was just created.
				DatabaseCreatedThisContext = response.StatusCode == (HttpStatusCode)201;

				if (DatabaseCreatedThisContext)
				{
					DbThroughput = Config.MaxRUPerSecond;
				}
			}

			if (Db == null)
			{
				Db = Client.GetDatabase(Config.DatabaseId);

				var dbTask = Db.ReadAsync();
				var throughputTask = Db.ReadThroughputAsync();

				await Task.WhenAll(dbTask, throughputTask).ConfigureAwait(false);

				var dbResponse = dbTask.Result;
				DbThroughput = throughputTask.Result ?? -1;
				DbProperties = dbResponse.Resource;
			}

			//Database Already Existed
			if (!DatabaseCreatedThisContext && !DbThroughput.HasValue)
			{
				DbThroughput = await Db.ReadThroughputAsync().ConfigureAwait(false) ?? -1;
			}

			return true;
		}

		/// <summary>
		/// Initializes the Cosmos tables using the provided <see cref="TableMetaData"/>.
		/// </summary>
		/// <param name="tableMetaDatas">The <see cref="TableMetaData"/> generated by the <see cref="Optimizer"/>.</param>
		/// <returns>A task for async completion.</returns>
		public async Task InitTables(ICollection<TableMetaData> tableMetaDatas)
		{
			var tasks = new List<Task<ContainerResponse>>();

			foreach (var tableMetaData in tableMetaDatas)
			{
				tasks.Add(Db.CreateContainerIfNotExistsAsync(new ContainerProperties(tableMetaData.TableName, tableMetaData.PartitionKeyPath)));
			}

			await Task.WhenAll(tasks).ConfigureAwait(false);

			var containers = tasks.Select(x => x.Result.Container).ToDictionary(x => x.Id, x => x);
			foreach (var tableMetaData in tableMetaDatas)
			{
				if (containers.ContainsKey(tableMetaData.TableName))
				{
					tableMetaData.CosmosContainer = containers[tableMetaData.TableName];
				}
				else
				{
					throw new DiiTableCreationFailedException(tableMetaData.TableName);
				}
			}

			TableMappings = tableMetaDatas.ToDictionary(x => x.ConcreteType, x => x);
		}
		#endregion Public Methods
	}
}