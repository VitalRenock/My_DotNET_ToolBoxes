using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Tools.Connections.Database
{
    public class Connection
    {
		#region Connection string

		private readonly string _connectionString;

		#endregion

		#region Constructors

		public Connection(string connectionString)
		{
			//Affectation
			_connectionString = connectionString;

			//Test de la connexion
			using (SqlConnection sqlConnection = new SqlConnection())
			{
				sqlConnection.ConnectionString = _connectionString;
				sqlConnection.Open();
			}
		}

		#endregion

		#region Private Methods
		
		/// <summary>
		/// Paramètre et retourne un objet 'SqlCommand' en fonction
		/// de la commande ('Command') et connexion ('SqlConnection') fournies.
		/// </summary>
		/// <param name="command">Commande paramètrée à transformée en objet 'SqlCommand'.</param>
		/// <param name="sqlConnection">Objet de connexion à la DB</param>
		/// <returns>Objet de type 'SqlCommand' paramètré.</returns>
        private SqlCommand CreateCommand(Command command, SqlConnection sqlConnection)
		{
			#region Création d'une nouvelle commande en utilisant la connexion fournie.

			SqlCommand newSqlCommand = sqlConnection.CreateCommand();

			#endregion

			#region Affectation de la requête

			newSqlCommand.CommandText = command._query;

			#endregion

			#region Gestion du cas d'une procédure stockée
			
			if (command._isStoredProcedure)
				newSqlCommand.CommandType = CommandType.StoredProcedure;

			#endregion

			#region Transmission des éventuels paramètres
			
			foreach (KeyValuePair<string, object> kvp in newSqlCommand.Parameters)
			{
				// Création d'un nouveau paramètre de type 'SqlParameter'.
				SqlParameter sqlParameter = new SqlParameter();

				// Affectation de la clé et de la valeur dans le nouveau 'SqlParameter'.
				sqlParameter.ParameterName = kvp.Key;
				sqlParameter.Value = kvp.Value;

				// Ajout du nouveau 'paramètre' dans la collection de 'paramètres'
				// de l'objet 'SqlCommand' à retourner
				newSqlCommand.Parameters.Add(sqlParameter);
			}

			#endregion

			return newSqlCommand;
		}

		/// <summary>
		/// Retourne un objet 'SqlConnection' avec la 'ConnectionString' définie.
		/// </summary>
		/// <returns>Objet de type 'SqlConnection'</returns>
		private SqlConnection CreateConnection()
		{
			SqlConnection sqlConnection = new SqlConnection();
			sqlConnection.ConnectionString = _connectionString;
			return sqlConnection;
		}

		/// <summary>
		/// Vérifie si la commande fournie n'est pas 'null', sinon remonte une erreur.
		/// </summary>
		/// <param name="commandToCheck">Commande à vérifier.</param>
		private void CheckNullCommand(Command commandToCheck)
		{
			// Si 'commandToCheck' est null, on remonte une erreur.
			if (commandToCheck is null)
				throw new ArgumentNullException(nameof(commandToCheck));
		}

		#endregion

		#region Public Methods

		// ??? Pour les Select?
		/// <summary>
		/// Méthode qui retourne une SEULE valeur de type 'object'.
		/// (Mode connecté)
		/// </summary>
		/// <param name="command">Commande déjà paramètrée</param>
		/// <returns>Le résultat de la requête sous forme 'object'</returns>
		public object ExecuteScalar(Command command)
		{
			#region Vérifications

			CheckNullCommand(command);

			#endregion

			// Création d'une nouvelle connexion,
			using (SqlConnection newSqlConnection = CreateConnection())
			{
				// Création d'une nouvelle commande,
				using (SqlCommand newSqlCommand = CreateCommand(command, newSqlConnection))
				{
					#region Ouverture de la connexion
					
					newSqlConnection.Open();

					#endregion

					#region Execution de la commande
					
					object result = newSqlCommand.ExecuteScalar();

					#endregion

					#region Retour du résultat
					
					// Si le résultat est de type 'DBNull', on retourne un 'null'.
					return (result is DBNull) ? null : result;

					#endregion
				}
			}
		}

		// ??? Pour les Select?
		/// <summary>
		/// Méthode qui retourne une TABLE de données de type 'IEnumerable<TResult>'.
		/// (Mode connecté)
		/// </summary>
		/// <typeparam name="TResult">Type d'objet désiré</typeparam>
		/// <param name="command">Commande déjà paramètrée</param>
		/// <param name="selector"></param>
		/// <returns>Retourne une collection de type IEnumerable<TResult> représentant la TABLE de données</returns>
		public IEnumerable<TResult> ExecuteReader<TResult>(Command command, Func<IDataRecord, TResult> selector)
		{
			#region Vérifications

			CheckNullCommand(command);

			// ??? Que représente 'selector'
			if (selector is null)
				throw new ArgumentNullException(nameof(selector));

			#endregion

			//Création d'une nouvelle connexion
			using (SqlConnection newSqlConnection = CreateConnection())
			{
				//Création d'une nouvelle commande.
				using (SqlCommand sqlCommand = CreateCommand(command, newSqlConnection))
				{
					#region Ouverture de la connexion

					newSqlConnection.Open();

					#endregion

					#region Exécution la méthode ExecuteReader sur l'object de type SqlCommand

					using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
					{
						// Consommation de l'objet de type 'SqlDataReader'.
						while (sqlDataReader.Read())
							yield return selector(sqlDataReader);
					}

					#endregion
				}
			}
		}

		// ??? Pour les Select?
		/// <summary>
		/// Méthode qui retourne une DATATABLE.
		/// (Mode déconnecté)
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public DataTable GetDataTable(Command command)
		{
			#region Vérifications

			CheckNullCommand(command);

			#endregion

			// Création d'une nouvelle connexion,
			using (SqlConnection newSqlConnection = CreateConnection())
			{
				// Création d'une nouvelle commande,
				using (SqlCommand newSqlCommand = CreateCommand(command, newSqlConnection))
				{
					#region Création d'un nouveau 'SqlDataAdapter'
					
					SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();

					#endregion

					#region Assignation de la commande dans le 'SqlDataAdaptater'

					sqlDataAdapter.SelectCommand = newSqlCommand;

					#endregion

					#region Création d'une nouvelle 'DataTable'

					DataTable dataTable = new DataTable();

					#endregion

					#region Exécution de la méthode Fill() du 'SqlDataAdapter' sur la 'DataTable'
					
					sqlDataAdapter.Fill(dataTable);

					#endregion

					return dataTable;
				}
			}
		}

		// ??? Pour les Insert, Update et Delete?
		/// <summary>
		/// Méthode qui execute la commande et retourne le nombre de lignes affectées.
		/// </summary>
		/// <param name="command">Commande à executer.</param>
		/// <returns>Retourne le nombre de lignes affectées.</returns>
		public int ExecuteNonQuery(Command command)
		{
			#region Vérifications

			// Si 'command' est 'null', on remonte une erreur.
			if (command is null)
				throw new ArgumentNullException(nameof(command));

			#endregion

			// Création d'une nouvelle connexion,
			using (SqlConnection newSqlConnection = CreateConnection())
			{
				// Création d'une nouvelle commande,
				using (SqlCommand newSqlCommand = CreateCommand(command, newSqlConnection))
				{
					#region Ouverture de la connexion
					
					newSqlConnection.Open();

					#endregion

					// ??? Gestion du 'DBNull' en 'null'?
					#region Execution de la commande
					
					return newSqlCommand.ExecuteNonQuery();

					#endregion
				}
			}
		}

		#region Obsolètes Scalaire

		//public object ExecuteScalar(Command command)
		//{
		//	// Vérifie si la 'Command' n'est pas 'null'
		//	if (command is null)
		//		throw new ArgumentNullException(nameof(command));

		//	//Crée une nouvelle connexion
		//	using (SqlConnection sqlConnection = new SqlConnection())
		//	{
		//		//Crée la commande
		//		using (SqlCommand sqlCommand = CreateCommand(command, sqlConnection))
		//		{
		//			// Ouvre la connexion.
		//			sqlConnection.Open();

		//			// Execution de la commande.
		//			object result = sqlCommand.ExecuteScalar();

		//			// On retourne le résultat, si le résultat est de type 'DBNull', on retourne un 'null'.
		//			return (result is DBNull) ? null : result;
		//		}
		//	}
		//}

		#endregion
		#region Obsolètes Reader

		//public IEnumerable<TResult> ExecuteReader1<TResult>(Command command, Func<IDataRecord, TResult> selector)
		//{
		//	// Si 'command' est 'null', on remonte une erreur.
		//	if (command is null)
		//		throw new ArgumentNullException(nameof(command));
		//	// ??? Que représente 'selector'
		//	if (selector is null)
		//		throw new ArgumentNullException(nameof(selector));

		//	//Création d'une nouvelle connexion
		//	using (SqlConnection sqlConnection = new SqlConnection())
		//	{
		//		sqlConnection.ConnectionString = _connectionString;

		//		//Création d'une nouvelle commande.
		//		using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
		//		{
		//			#region Affectation de la requête

		//			sqlCommand.CommandText = command._query;

		//			#endregion

		//			#region Gestion du cas d'une procédure stockée

		//			if (command._isStoredProcedure)
		//				sqlCommand.CommandType = CommandType.StoredProcedure;

		//			#endregion

		//			#region Transmission des éventuels paramètres

		//			foreach (KeyValuePair<string, object> kvp in command._parameters)
		//			{
		//				SqlParameter sqlParameter = new SqlParameter();
		//				sqlParameter.ParameterName = kvp.Key;
		//				sqlParameter.Value = kvp.Value;
		//				sqlCommand.Parameters.Add(sqlParameter);
		//			}

		//			#endregion

		//			#region Ouverture de la connexion

		//			sqlConnection.Open();

		//			#endregion

		//			#region Exécution la méthode ExecuteReader sur l'object de type SqlCommand

		//			using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
		//			{
		//				// Consommation de l'objet de type 'SqlDataReader'.
		//				while (sqlDataReader.Read())
		//					yield return selector(sqlDataReader);
		//			}

		//			#endregion
		//		}
		//	}
		//}
		//public IEnumerable<TResult> ExecuteReader2<TResult>(Command command, Func<IDataRecord, TResult> selector)
		//{
		//	List<TResult> results = new List<TResult>();

		//	using (SqlConnection connection = CreateConnection())
		//	{
		//		using (SqlCommand sqlCommand = CreateCommand(command, connection))
		//		{
		//			connection.Open();

		//			using (SqlDataReader reader = sqlCommand.ExecuteReader())
		//			{
		//				while (reader.Read())
		//				{
		//					yield return selector(reader);
		//				}
		//			}
		//		}
		//	}
		//}

		#endregion
		#region Obsolètes GetDataTable
		
		//public DataTable GetDataTable1(Command command)
		//{
		//	// Vérifier si la 'command' n'est pas null
		//	if (command is null)
		//		throw new ArgumentNullException(nameof(command));

		//	//Crée la connexion
		//	using (SqlConnection sqlConnection = new SqlConnection())
		//	{
		//		sqlConnection.ConnectionString = _connectionString;

		//		//Crée la commande
		//		using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
		//		{
		//			sqlCommand.CommandText = command._query;

		//			//Crée le SqlDataAdapter
		//			SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
		//			sqlDataAdapter.SelectCommand = sqlCommand;

		//			//Crée la DataTable
		//			DataTable dataTable = new DataTable();

		//			//J'exécute la méthode Fill sur l'object de type SqlDataAdapter
		//			sqlDataAdapter.Fill(dataTable);

		//			return dataTable;
		//		}
		//	}
		//}
		//public DataTable GetDataTable2(Command command)
		//{
		//	// Vérifier si la 'command' n'est pas null
		//	if (command is null)
		//		throw new ArgumentNullException(nameof(command));

		//	//Crée la connexion
		//	using (SqlConnection sqlConnection = new SqlConnection())
		//	{
		//		//Crée la commande
		//		using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
		//		{
		//			//Crée le SqlDataAdapter
		//			SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
		//			sqlDataAdapter.SelectCommand = sqlCommand;

		//			//Crée la DataTable
		//			DataTable dataTable = new DataTable();

		//			//J'exécute la méthode Fill sur l'object de type SqlDataAdapter
		//			sqlDataAdapter.Fill(dataTable);

		//			return dataTable;
		//		}
		//	}
		//}

		#endregion
		#region Obsolètes NonQuery

		//public int ExecuteNonQuery1(Command command)
		//{
		//	// Vérifier si la 'command' n'est pas null
		//	if (command is null)
		//		throw new ArgumentNullException(nameof(command));

		//	//Crée la connexion (SqlConnection)
		//	using (SqlConnection sqlConnection = new SqlConnection())
		//	{
		//		sqlConnection.ConnectionString = _connectionString;

		//		//Crée la commande (SqlCommand)
		//		using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
		//		{
		//			sqlCommand.CommandText = command._query;

		//			//J'ouvre la connexion
		//			sqlConnection.Open();

		//			//J'exécute la méthode ExecuteNonQuery sur l'object de type SqlCommand
		//			return sqlCommand.ExecuteNonQuery();
		//		}
		//	}
		//}
		//public int ExecuteNonQuery2(Command command)
		//{
		//	// Vérifier si la 'command' n'est pas null
		//	if (command is null)
		//		throw new ArgumentNullException(nameof(command));

		//	//Crée la connexion
		//	using (SqlConnection sqlConnection = new SqlConnection())
		//	{
		//		//Crée la commande
		//		using (SqlCommand sqlCommand = CreateCommand(command, sqlConnection))
		//		{
		//			sqlConnection.Open();
		//			return sqlCommand.ExecuteNonQuery();
		//		}
		//	}
		//}

		#endregion

		#endregion
	}
}