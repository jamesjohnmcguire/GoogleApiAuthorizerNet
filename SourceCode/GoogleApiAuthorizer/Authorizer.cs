// <copyright file="Authorizer.cs" company="Digital Zen Works">
// Copyright © 2022 Digital Zen Works. All Rights Reserved.
// </copyright>

using Common.Logging;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

[assembly: CLSCompliant(false)]

namespace DigitalZenWorks.GoogleApiAuthorizer
{
	/// <summary>
	/// Provides support for authorizing clients with Google APIs services.
	/// </summary>
	public class Authorizer
	{
		private static readonly ILog Log = LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private readonly IList<string> scopes = new List<string>();

		/// <summary>
		/// Initializes a new instance of the <see cref="Authorizer"/> class.
		/// </summary>
		/// <param name="name">The name of the project requesting
		/// authorization.</param>
		/// <param name="scopes">The requested scopes of the project.</param>
		/// <param name="promptUser">A value indicating whether to prompt
		/// the user or not.</param>
		public Authorizer(
			string name,
			IList<string> scopes,
			bool promptUser)
		{
			this.Name = name;

			if (scopes != null)
			{
				foreach (string scope in scopes)
				{
					this.Scopes.Add(scope);
				}
			}

			PromptUser = promptUser;
		}

		/// <summary>
		/// Gets or sets the name property.
		/// </summary>
		/// <value>The name property.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets the scopes property.
		/// </summary>
		/// <value>The scopes property.</value>
		public IList<string> Scopes { get { return scopes; } }

		/// <summary>
		/// Gets or sets a value indicating whether the prompt user property.
		/// </summary>
		/// <value>A value indicating whether to prompt the user or not.</value>
		public bool PromptUser { get; set; }

		/// <summary>
		/// Main static method for authorization.
		/// </summary>
		/// <param name="mode">The type of authorization process.</param>
		/// <param name="credentialsFilePath">The standard project credentials
		/// json file.</param>
		/// <param name="serviceAccountFilePath">The service account
		/// credentials json file.</param>
		/// <param name="tokensFilePath">The tokens json file.</param>
		/// <param name="name">The name of the project requesting
		/// authorization.</param>
		/// <param name="scopes">The requested scopes of the project.</param>
		/// <param name="redirectUrl">The URL which the authorization will
		/// complete to.</param>
		/// <param name="promptUser">A value indicating whether to prompt the user or not.</param>
		/// <returns>A google API client object.</returns>
		public static BaseClientService.Initializer Authorize(
			Mode mode,
			string credentialsFilePath,
			string serviceAccountFilePath,
			string tokensFilePath,
			string name,
			IList<string> scopes,
			Uri redirectUrl = null,
			bool promptUser = true)
		{
			BaseClientService.Initializer client = AuthorizeByMode(
				mode,
				credentialsFilePath,
				serviceAccountFilePath,
				tokensFilePath,
				name,
				scopes,
				redirectUrl,
				promptUser);

			// Final fall back, prompt user for confirmation code through web page.
			client = FinalFallBack(
				client,
				credentialsFilePath,
				tokensFilePath,
				name,
				scopes,
				redirectUrl,
				promptUser);

			return client;
		}

		/// <summary>
		/// Authorize by OAuth method.
		/// </summary>
		/// <param name="credentialsFilePath">The standard project credentials
		/// json file.</param>
		/// <param name="name">The name of the project requesting
		/// authorization.</param>
		/// <param name="scopes">The requested scopes of the project.</param>
		/// <param name="redirectUrl">The URL which the authorization will
		/// complete to.</param>
		/// <returns>A google API client object.</returns>
		public static BaseClientService.Initializer AuthorizeOauth(
			string credentialsFilePath,
			string name,
			IList<string> scopes,
			Uri redirectUrl)
		{
			BaseClientService.Initializer client = null;

			return client;
		}

		/// <summary>
		/// Authorize by service account method.
		/// </summary>
		/// <param name="serviceAccountFilePath">The service account
		/// credentials json file.</param>
		/// <param name="name">The name of the project requesting
		/// authorization.</param>
		/// <param name="scopes">The requested scopes of the project.</param>
		/// <returns>A google API client object.</returns>
		public static BaseClientService.Initializer AuthorizeServiceAccount(
			string serviceAccountFilePath,
			string name,
			IList<string> scopes)
		{
			BaseClientService.Initializer baseClient = null;
			GoogleCredential credentialedAccount = null;

			if (!string.IsNullOrEmpty(serviceAccountFilePath) &&
				File.Exists(serviceAccountFilePath))
			{
				credentialedAccount =
					GoogleCredential.FromFile(serviceAccountFilePath);
			}
			else
			{
				// Attempt to gain access from file specified in the
				// GOOGLE_APPLICATION_CREDENTIALS environment variable.
				string environmentVariable =
					Environment.GetEnvironmentVariable(
						"GOOGLE_APPLICATION_CREDENTIALS");
				if (environmentVariable != null)
				{
					try
					{
						credentialedAccount =
							GoogleCredential.GetApplicationDefault();
					}
					catch (Exception exception) when
					(exception is DirectoryNotFoundException ||
					exception is FileNotFoundException ||
					exception is InvalidOperationException ||
					exception is AggregateException ||
					exception is System.Reflection.TargetInvocationException)
					{
						Log.Error(exception.ToString());
					}
				}
			}

			if (credentialedAccount == null)
			{
				Log.Warn("WARNING: Service account credentials not set");
			}
			else
			{
				credentialedAccount = credentialedAccount.CreateScoped(scopes);

				baseClient = new BaseClientService.Initializer();
				baseClient.ApplicationName = "Backup Manager";
				baseClient.HttpClientInitializer = credentialedAccount;
			}

			return baseClient;
		}

		/// <summary>
		/// Authorize by tokens method.
		/// </summary>
		/// <param name="credentialsFilePath">The standard project credentials
		/// json file.</param>
		/// <param name="tokensFilePath">The tokens json file.</param>
		/// <param name="name">The name of the project requesting
		/// authorization.</param>
		/// <param name="scopes">The requested scopes of the project.</param>
		/// <returns>A google API client object.</returns>
		public static BaseClientService.Initializer AuthorizeToken(
			string credentialsFilePath,
			string tokensFilePath,
			string name,
			IList<string> scopes)
		{
			BaseClientService.Initializer client = null;
			Tokens accessToken = AuthorizeTokenFile(tokensFilePath);

			if (accessToken == null)
			{
				accessToken = AuthorizeTokenLocal();
			}

			if (accessToken != null)
			{
				client = SetClient(
					credentialsFilePath,
					name,
					scopes,
					true);

				client = SetAccessToken(
					client,
					accessToken,
					tokensFilePath);
			}

			return client;
		}

		/// <summary>
		/// Prompt for authorization code CLI method.
		/// </summary>
		/// <param name="credentialsFilePath">The standard project credentials
		/// json file.</param>
		/// <param name="tokensFilePath">The tokens json file.</param>
		/// <param name="name">The name of the project requesting
		/// authorization.</param>
		/// <param name="scopes">The requested scopes of the project.</param>
		/// <returns>A google API client object.</returns>
		public static BaseClientService.Initializer RequestAuthorization(
			string credentialsFilePath,
			string tokensFilePath,
			string name,
			IList<string> scopes)
		{
			BaseClientService.Initializer baseClient = null;

			if (Environment.UserInteractive == false)
			{
				Log.Warn("WARNING: Requesting user authorization only " +
					"works at the command line");
			}
			else
			{
				baseClient = SetClient(
					credentialsFilePath,
					name,
					scopes,
					true);

				if (baseClient != null)
				{
					// Uri authorizationUrl = baseClient.createAuthUrl();
					// string authorizationCode =
					// PromptForAuthorizationCodeCli(authorizationUrl);

					// Tokens accessToken =
					// baseClient.fetchAccessTokenWithAuthCode(authorizationCode);
					// baseClient = SetAccessToken(
					// baseClient,
					// accessToken,
					// tokensFilePath);
				}
			}

			return baseClient;
		}

		/// <summary>
		/// Authorize method.
		/// </summary>
		/// <param name="mode">The type of authorization to process.</param>
		/// <param name="credentialsFilePath">The standard project credentials
		/// json file.</param>
		/// <param name="serviceAccountFilePath">The service account
		/// credentials json file.</param>
		/// <param name="tokensFilePath">The tokens json file.</param>
		/// <param name="redirectUrl">The URL which the authorization will
		/// complete to.</param>
		/// <returns>A google API client object.</returns>
		public BaseClientService.Initializer Authorize(
			Mode mode,
			string credentialsFilePath,
			string serviceAccountFilePath,
			string tokensFilePath,
			Uri redirectUrl = null)
		{
			BaseClientService.Initializer client = Authorize(
				mode,
				credentialsFilePath,
				serviceAccountFilePath,
				tokensFilePath,
				Name,
				Scopes,
				redirectUrl,
				PromptUser);

			return client;
		}

		/// <summary>
		/// Authorize by OAuth method.
		/// </summary>
		/// <param name="credentialsFilePath">The standard project credentials
		/// json file.</param>
		/// <param name="redirectUrl">The URL which the authorization will
		/// complete to.</param>
		/// <returns>A google API client object.</returns>
		public BaseClientService.Initializer AuthorizeOauth(
			string credentialsFilePath,
			Uri redirectUrl)
		{
			BaseClientService.Initializer client = AuthorizeOauth(
				credentialsFilePath,
				Name,
				Scopes,
				redirectUrl);

			return client;
		}

		/// <summary>
		/// Authorize by service account method.
		/// </summary>
		/// <param name="serviceAccountFilePath">The service account
		/// credentials json file.</param>
		/// <returns>A google API client object.</returns>
		public BaseClientService.Initializer AuthorizeServiceAccount(
			string serviceAccountFilePath)
		{
			BaseClientService.Initializer client = AuthorizeServiceAccount(
				serviceAccountFilePath,
				Name,
				Scopes);

			return client;
		}

		/// <summary>
		/// Authorize by tokens method.
		/// </summary>
		/// <param name="credentialsFilePath">The standard project credentials
		/// json file.</param>
		/// <param name="tokensFilePath">The tokens json file.</param>
		/// <returns>A google API client object.</returns>
		public BaseClientService.Initializer AuthorizeToken(
			string credentialsFilePath,
			string tokensFilePath)
		{
			BaseClientService.Initializer client = AuthorizeToken(
				credentialsFilePath,
				tokensFilePath,
				Name,
				Scopes);

			return client;
		}

		/// <summary>
		/// Prompt for authorization code CLI method.
		/// </summary>
		/// <param name="credentialsFilePath">The standard project credentials
		/// json file.</param>
		/// <param name="tokensFilePath">The tokens json file.</param>
		/// <returns>A google API client object.</returns>
		public BaseClientService.Initializer RequestAuthorization(
			string credentialsFilePath,
			string tokensFilePath)
		{
			BaseClientService.Initializer client = RequestAuthorization(
				credentialsFilePath,
				tokensFilePath,
				Name,
				Scopes);

			return client;
		}

		private static BaseClientService.Initializer AuthorizeByMode(
			Mode mode,
			string credentialsFilePath,
			string serviceAccountFilePath,
			string tokensFilePath,
			string name,
			IList<string> scopes,
			Uri redirectUrl,
			bool promptUser)
		{
			BaseClientService.Initializer client = null;

			switch (mode)
			{
				case Mode.Discover:
					client = AuthorizeToken(
						credentialsFilePath,
						tokensFilePath,
						name,
						scopes);

					if (client == null)
					{
						client = AuthorizeServiceAccount(
							serviceAccountFilePath,
							name,
							scopes);

						// Http fall back, redirect user for confirmation.
						if (client == null && promptUser == true)
						{
							client = RequestAuthorization(
								credentialsFilePath,
								tokensFilePath,
								name,
								scopes);
						}

						// Else use final fall back.
					}

					break;
				case Mode.OAuth:
					client = AuthorizeOauth(
						credentialsFilePath,
						name,
						scopes,
						redirectUrl);
					break;
				case Mode.Request:
					client = RequestAuthorization(
						credentialsFilePath,
						tokensFilePath,
						name,
						scopes);
					break;
				case Mode.ServiceAccount:
					client = AuthorizeServiceAccount(
						serviceAccountFilePath,
						name,
						scopes);
					break;
				case Mode.Token:
					client = AuthorizeToken(
						credentialsFilePath,
						tokensFilePath,
						name,
						scopes);
					break;
				default:
					// Use final fall back.
					break;
			}

			return client;
		}

		private static Tokens AuthorizeTokenLocal()
		{
			// Last chance attempt of hard coded file name.
			string tokenFilePath = "token.json";

			Tokens accessToken = AuthorizeTokenFile(tokenFilePath);

			return accessToken;
		}

		private static Tokens AuthorizeTokenFile(
			string tokensFilePath)
		{
			Tokens tokens = null;
			bool exists = false;

			if (tokensFilePath != null)
			{
				exists = File.Exists(tokensFilePath);

				if (exists == true)
				{
					string fileContents = File.ReadAllText(tokensFilePath);
					tokens =
						JsonConvert.DeserializeObject<Tokens>(fileContents);
				}
			}

			if (exists == false)
			{
				Log.Warn("WARNING: token file doesn't exist - " + tokensFilePath);
			}

			return tokens;
		}

		private static BaseClientService.Initializer FinalFallBack(
			BaseClientService.Initializer client,
			string credentialsFilePath,
			string tokensFilePath,
			string name,
			IList<string> scopes,
			Uri redirectUrl,
			bool promptUser)
		{
			if (client == null && promptUser == true)
			{
				if (Environment.UserInteractive == true)
				{
					client = RequestAuthorization(
						credentialsFilePath,
						tokensFilePath,
						name,
						scopes);
				}
				else
				{
					client = AuthorizeOauth(
						credentialsFilePath,
						name,
						scopes,
						redirectUrl);
				}
			}

			return client;
		}

		private static string PromptForAuthorizationCodeCli(
			Uri authorizationUrl)
		{
			Console.WriteLine("Open the following link in your browser:");
			Console.WriteLine(authorizationUrl.ToString());
			Console.WriteLine("Enter verification code:");
			string authorizationCode = Console.ReadLine();
			authorizationCode = authorizationCode.Trim();

			return authorizationCode;
		}

		private static BaseClientService.Initializer SetAccessToken(
			BaseClientService.Initializer client,
			Tokens tokens,
			string tokensFilePath)
		{
			BaseClientService.Initializer updatedClient = null;
			bool errorExists = false;

			if (tokens != null)
			{
				if (!string.IsNullOrWhiteSpace(tokens.Error))
				{
					// client.setAccessToken(tokens);
					updatedClient = client;

					string json = JsonConvert.SerializeObject(tokens);

					File.WriteAllText(tokensFilePath, json);
				}
				else
				{
					errorExists = true;
				}
			}

			if (updatedClient == null)
			{
				if (tokens == null)
				{
					Log.Warn("Tokens is null");
				}
				else if (errorExists == true)
				{
					Log.Warn("Error key exists in tokens");
				}
				else
				{
					Log.Warn("Problem with tokens object");
				}
			}

			return updatedClient;
		}

		private static BaseClientService.Initializer SetClient(
			string credentialsFilePath,
			string name,
			IList<string> scopes,
			bool credentialsRequired)
		{
			BaseClientService.Initializer baseClient = null;
			bool exists = false;

			if (!string.IsNullOrEmpty(credentialsFilePath) &&
				File.Exists(credentialsFilePath))
			{
				exists = true;
			}

			if (credentialsRequired == true && exists == false)
			{
				Log.Warn("Credentials not found");
			}
			else
			{
				BaseClientService.Initializer initializer = new ();
				initializer.ApplicationName = name;

				if (credentialsRequired == true)
				{
					try
					{
						GoogleCredential credentialedAccount =
							GoogleCredential.FromFile(credentialsFilePath);
						credentialedAccount =
							credentialedAccount.CreateScoped(scopes);

						initializer.HttpClientInitializer = credentialedAccount;

						baseClient = initializer;
					}
					catch (InvalidOperationException exception)
					{
						Log.Error(exception.ToString());
					}
				}

				// baseClient.setAccessType("offline");
				// baseClient.setPrompt("select_account consent");

				// if (exists == true)
				// {
				// baseClient.setAuthConfig(credentialsFilePath);
				// }
			}

			return baseClient;
		}
	}
}
