using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VitalTools.Database
{
	public class Connection : IConnection
	{
		#region Connection Properties
		
		private DbProviderFactory dbProvider;
		private string connectionString;

		#endregion

		#region Constructor
		
		public Connection(DbProviderFactory dbProvider, string connectionString)
		{
			#region Vérifications
			
			if (string.IsNullOrWhiteSpace(connectionString))
				throw new ArgumentException("'connectionString' ne peut être 'null' ou vide.");

			if (dbProvider is null)
				throw new ArgumentNullException(nameof(dbProvider));

			#endregion

			#region Assignations
			
			this.dbProvider = dbProvider;
			this.connectionString = connectionString;

			#endregion

			#region Test de la connexion

			using (DbConnection testConnection = CreateConnection())
			{
				try
				{
					testConnection.Open();
				}
				catch (Exception)
				{
					throw new InvalidOperationException("'connectionString' isn't valid or the server is not started!!");
				}
			}

			#endregion
		}

		#endregion

		#region Private Methods
		
		private DbConnection CreateConnection()
		{
			#region Vérifications

			if (dbProvider is null)
				throw new ArgumentNullException("Le 'dbProvider' ne peut être 'null'");

			if (string.IsNullOrWhiteSpace(connectionString))
				throw new ArgumentNullException("La chaine de connexion ne peut être 'null', vide ou être composé uniquement d'espaces blancs.");

			#endregion

			#region Création d'une nouvelle connexion

			DbConnection newConnection = dbProvider.CreateConnection();
			newConnection.ConnectionString = connectionString;

			#endregion

			return newConnection;
		}
		private DbCommand CreateCommand(Command command, DbConnection connection)
		{
			#region Vérifications 
			
			if (command is null)
				throw new ArgumentNullException(nameof(command));

			#endregion

			DbCommand dbCommand = connection.CreateCommand();

			#region Gestion de la requête
			
			dbCommand.CommandText = command.Query;

			#endregion

			#region Gestion d'une procédure stockée

			if (command.IsStoredProcedure)
				dbCommand.CommandType = CommandType.StoredProcedure;

			#endregion

			#region Gestion des paramètres de la requête
			
			foreach (KeyValuePair<string, object> kvp in command.Parameters)
			{
				DbParameter sqlParameter = dbCommand.CreateParameter();
				sqlParameter.ParameterName = kvp.Key;
				sqlParameter.Value = kvp.Value;

				dbCommand.Parameters.Add(sqlParameter);
			}

			#endregion

			return dbCommand;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Exécute la requête et retourne la première colonne de la première ligne du jeu
		/// de résultats retourné par la requête. Toutes les autres colonnes et lignes sont
		/// ignorées.
		/// </summary>
		/// <param name="command">Commande à exécuter</param>
		/// <returns>La première colonne de la première ligne du résultat défini.</returns>
		public object ExecuteScalar(Command command)
		{
			// Création d'une nouvelle connexion
			using (DbConnection dbConnection = CreateConnection())
			{
				// Création d'une nouvelle commande
				using (DbCommand dbCommand = CreateCommand(command, dbConnection))
				{
					// Ouverture de la connexion
					dbConnection.Open();

					// Execution de la commande
					object result = dbCommand.ExecuteScalar();

					// Retour du résultat, si le résultat est de type 'DBNull', on retourne un 'null'.
					return (result is DBNull) ? null : result;
				}
			}
		}

		/// <summary>
		/// Exécute la requête et retourne une liste de résultat en fonction du sélecteur.
		/// </summary>
		/// <typeparam name="TResult">Type de la colonne</typeparam>
		/// <param name="command">Commande à exécuter</param>
		/// <param name="selector">Sélecteur</param>
		/// <returns>Retourne IEnumerable<TResult></returns>
		public IEnumerable<TResult> ExecuteReader<TResult>(Command command, Func<IDataRecord, TResult> selector)
		{
			if (selector is null)
				throw new ArgumentNullException(nameof(selector));

			// Création d'une nouvelle connexion
			using (DbConnection dbConnection = CreateConnection())
			{
				// Création d'une nouvelle commande
				using (DbCommand dbCommand = CreateCommand(command, dbConnection))
				{
					// Ouverture de la connexion
					dbConnection.Open();

					#region Exécution la méthode ExecuteReader sur l'object de type SqlCommand

					using (DbDataReader reader = dbCommand.ExecuteReader())
					{
						// Consommation de l'objet de type 'SqlDataReader'.
						while (reader.Read())
							yield return selector(reader);
					}

					#endregion
				}
			}
		}
		/// <summary>
		/// Exécute la requête et retourne un DbDataReader.
		/// </summary>
		/// <param name="command">Commande à exécuter</param>
		/// <returns>Retourne un DbDataReader</returns>
		public DbDataReader ExecuteReader(Command command)
		{
			// Création d'une nouvelle connexion
			using (DbConnection dbConnection = CreateConnection())
			{
				// Création d'une nouvelle commande
				using (DbCommand dbCommand = CreateCommand(command, dbConnection))
				{
					// Ouverture de la connexion
					dbConnection.Open();

					// Execution de la commande
					return dbCommand.ExecuteReader();
				}
			}
		}

		/// <summary>
		/// Exécute la requête et retourne une DataTable.
		/// </summary>
		/// <param name="command">Commande à exécuter</param>
		/// <returns>Retourne une DataTable</returns>
		public DataTable GetDataTable(Command command)
		{
			// Création d'une nouvelle connexion
			using (DbConnection dbConnection = CreateConnection())
			{
				// Création d'une nouvelle commande
				using (DbCommand dbCommand = CreateCommand(command, dbConnection))
				{
					// Création d'un nouveau 'SqlDataAdapter' avec le 'SqlCommand' à executer.
					DbDataAdapter dataAdapter = dbProvider.CreateDataAdapter();

					// Création d'une nouvelle 'DataTable'
					DataTable datatable = new DataTable();

					// Exécution de la méthode Fill() du 'SqlDataAdapter' sur la 'DataTable'
					dataAdapter.Fill(datatable);

					return datatable;
				}
			}
		}

		/// <summary>
		/// Exécute la requête et retourne le nombre de ligne affectées.
		/// </summary>
		/// <param name="command">Commande à exécuter</param>
		/// <returns>Retourne le nombre de ligne affectées</returns>
		public int ExecuteNonQuery(Command command)
		{
			// Création d'une nouvelle connexion
			using (DbConnection dbConnection = CreateConnection())
			{
				// Création d'une nouvelle commande
				using (DbCommand dbCommand = CreateCommand(command, dbConnection))
				{
					// Ouverture de la connexion
					dbConnection.Open();

					// Execution de la commande
					return dbCommand.ExecuteNonQuery();
				}
			}
		}

		#endregion
	}
}