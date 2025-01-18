// <copyright file="Tokens.cs" company="Digital Zen Works">
// Copyright © 2022 - 2025 Digital Zen Works. All Rights Reserved.
// </copyright>

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace DigitalZenWorks.GoogleApiAuthorizer
{
	/// <summary>
	/// Represents a set of OAuth tokens.
	/// </summary>
	public class Tokens
	{
		/// <summary>
		/// Gets or sets the access token property.
		/// </summary>
		/// <value>The access token property.</value>
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		/// <summary>
		/// Gets or sets the refresh token property.
		/// </summary>
		/// <value>The refresh token property.</value>
		[JsonProperty("refresh_token")]
		public string RefreshToken { get; set; }

		/// <summary>
		/// Gets or sets the error property.
		/// </summary>
		/// <value>The error property.</value>
		public string Error { get; set; }

		/// <summary>
		/// Gets or sets the scope property.
		/// </summary>
		/// <value>The scope property.</value>
		public string Scope { get; set; }

		/// <summary>
		/// Gets or sets the token type property.
		/// </summary>
		/// <value>The token type property.</value>
		[JsonProperty("token_type")]
		public string TokenType { get; set; }

		/// <summary>
		/// Gets or sets the created property.
		/// </summary>
		/// <value>The created property.</value>
		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime Created { get; set; }
	}
}
