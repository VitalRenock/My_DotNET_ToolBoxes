using System;
using System.Collections.Generic;
using System.Text;

namespace Tools.Connections.Database
{
    public class Command
    {
		#region Properties

		internal string _query { get; private set; }
		internal bool _isStoredProcedure { get; private set; }
		internal IDictionary<string, object> _parameters { get; private set; }

		#endregion

		#region Constructors
		
		public Command(string query, bool isStoredProcedure = false)
		{
			// Si 'query' est 'null' ou vide, on remonte une erreur.
			if (query == null || query.Trim().Length == 0)
				throw new ArgumentException("Query can't be null");
			// Sinon on instancie les propriétés.
			else
			{
				_query = query;
				_parameters = new Dictionary<string, object>();
				_isStoredProcedure = isStoredProcedure;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Ajoute un paramètre au dictionnaire de paramètres de la commande.
		/// </summary>
		/// <param name="parameterName">Clé du nouveau paramètre. (ne peut être 'null' ou  vide)</param>
		/// <param name="value">Valeur du nouveau paramètre. (ne peut être 'null')</param>
		public void AddParameter(string parameterName, object value)
		{
			// Si 'parameterName' est 'null' ou vide, on remonte une erreur.
			if (parameterName == null || parameterName.Trim().Length == 0)
				throw new ArgumentNullException("Parameters can't be bull");
			// Sinon si 'value' est 'null', on remonte une erreur.
			else if (value == null)
				throw new NullReferenceException("Object value can't be null");
			// Sinon si le dictionnaire '_parameters' contient déjà un 'parameterName' identique, on remonte une erreur.
			else if (_parameters.ContainsKey(parameterName))
				throw new MissingMemberException("Parameter {0} already exist", parameterName);

			// ATTENTION: Coalesce contradictoire avec le 1er 'else if' plus haut!!!
			// Ajoute au dictionnaire '_parameters' le nouveau paramètre en transformant
			// les valeurs 'null' en 'DBNull'.
			_parameters.Add(parameterName, value ?? DBNull.Value);
		} 
		
		#endregion
	}
}