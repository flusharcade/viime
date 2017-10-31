// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StocklistRepository.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2015 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Camera.Portable.WebServices
{
	using System;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Reactive.Linq;
	using System.Collections.Generic;
	using System.Threading;

	using Newtonsoft.Json;

	/// <summary>
	/// Stocklist web service controller.
	/// </summary>
	public sealed class StocklistWebServiceController
	{
		#region Fields

		/// <summary>
		/// The client handler.
		/// </summary>
		private readonly HttpClientHandler _clientHandler;

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Gets all stock items.
		/// </summary>
		/// <returns>The all stock items.</returns>
		public IObservable<List<StockItemContract>> GetAllStockItems ()
		{
			var authClient = new HttpClient (_clientHandler);

			var message = new HttpRequestMessage (HttpMethod.Get, new Uri (""));

			return Observable.FromAsync(() => authClient.SendAsync (message, new CancellationToken(false)))
				.SelectMany(async response => 
					{
						if (response.StatusCode != HttpStatusCode.OK)
						{
							throw new Exception("Respone error");
						}

						return await response.Content.ReadAsStringAsync();
					})
				.Select(json => JsonConvert.DeserializeObject<List<StockItemContract>>(json));
		}

		/// <summary>
		/// Gets the stock item.
		/// </summary>
		/// <returns>The stock item.</returns>
		/// <param name="id">Identifier.</param>
		public IObservable<StockItemContract> GetStockItem(int id)
		{
			var authClient = new HttpClient(_clientHandler);

            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(""));

			return Observable.FromAsync(() => authClient.SendAsync(message, new CancellationToken(false)))
				.SelectMany(async response =>
					{
						if (response.StatusCode != HttpStatusCode.OK)
						{
							throw new Exception("Respone error");
						}

						return await response.Content.ReadAsStringAsync();
					})
				.Select(json => JsonConvert.DeserializeObject<StockItemContract>(json));
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="T:Stocklist.Portable.Repositories.StocklistWebServiceController.GeocodingRepository"/> class.
		/// </summary>
		/// <param name="clientHandler">Client handler.</param>
		public StocklistWebServiceController(HttpClientHandler clientHandler)
		{
			_clientHandler = clientHandler;
		}

		#endregion
	}
}