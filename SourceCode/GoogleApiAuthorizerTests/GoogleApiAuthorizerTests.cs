// <copyright file="GoogleApiAuthorizerTests.cs" company="Digital Zen Works">
// Copyright © 2022 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>

using DigitalZenWorks.GoogleApiAuthorizer;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using NUnit.Framework;
using System;

[assembly: CLSCompliant(true)]

namespace DigitalZenWorks.GoogleApiAuthorizer.Tests
{
	/// <summary>
	/// Test class for GoogleApiAuthorizer.
	/// </summary>
	public class GoogleApiAuthorizerTests
	{
		private string credentialsFilePath;
		private string serviceAccountFilePath;
		private string[] scopes = { "https://www.googleapis.com/auth/drive" };
		private string tokensFilePath;

		/// <summary>
		/// One time set up method.
		/// </summary>
		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			string baseDataDirectory = Environment.GetFolderPath(
				Environment.SpecialFolder.ApplicationData,
				Environment.SpecialFolderOption.Create);
			string dataPath = baseDataDirectory +
				@"\DigitalZenWorks\GoogleApiAuthorizer";
			credentialsFilePath = dataPath + @"\Credentials.json";
			serviceAccountFilePath = dataPath + @"\ServiceAccount.json";
			tokensFilePath = dataPath + @"\Tokens.json";
		}

		/// <summary>
		/// One time tear down method.
		/// </summary>
		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
		}

		/// <summary>
		/// Set up method.
		/// </summary>
		[SetUp]
		public void Setup()
		{
		}

		/// <summary>
		/// Sanity Check Test.
		/// </summary>
		[Test]
		public void SanityCheck()
		{
			Assert.Pass();
		}

		/// <summary>
		/// Discover Fail Test.
		/// </summary>
		[Test]
		public void DiscoverFail()
		{
			BaseClientService.Initializer client = Authorizer.Authorize(
				Mode.Discover,
				null,
				null,
				null,
				"Google Drive API File Uploader",
				scopes);

			Assert.Null(client);
		}

		/// <summary>
		/// Discover Object Fail Test.
		/// </summary>
		[Test]
		public void DiscoverObjectFail()
		{
			Authorizer authorizer =
				new ("Google Drive API File Uploader", scopes, false);

			BaseClientService.Initializer client = authorizer.Authorize(
				Mode.Discover,
				null,
				null,
				null);

			Assert.Null(client);
		}

		/// <summary>
		/// Service Account Direct No File Or Environement Variable Fail Test.
		/// </summary>
		[Test]

		public void ServiceAccountDirectNoFileOrEnvironementVariableFail()
		{
			string environmentVariable = "GOOGLE_APPLICATION_CREDENTIALS";
			Environment.SetEnvironmentVariable(environmentVariable, null);

			BaseClientService.Initializer client =
				Authorizer.AuthorizeServiceAccount(
					string.Empty,
					"Google Drive API File Uploader",
					scopes);

			Assert.Null(client);
		}

		/// <summary>
		/// Service Account Direct Environment Variable Success Test.
		/// </summary>
		[Test]

		public void ServiceAccountDirectEnvironmentVariableSuccess()
		{
			string environmentVariable = "GOOGLE_APPLICATION_CREDENTIALS";
			Environment.SetEnvironmentVariable(
				environmentVariable, serviceAccountFilePath);

			BaseClientService.Initializer client =
				Authorizer.AuthorizeServiceAccount(
					null,
					"Google Drive API File Uploader",
					scopes);

			Assert.NotNull(client);

			using DriveService driveService = new (client);

			var about = driveService.About;

			var response = about.Get();
			Assert.NotNull(response);

			Assert.IsInstanceOf<Google.Apis.Drive.v3.AboutResource.GetRequest>(
				response);
		}

		/// <summary>
		/// Service Account Direct File Success Test.
		/// </summary>
		[Test]

		public void ServiceAccountDirectFileSuccess()
		{
			BaseClientService.Initializer client =
				Authorizer.AuthorizeServiceAccount(
					serviceAccountFilePath,
					"Google Drive API File Uploader",
					scopes);

			Assert.NotNull(client);

			using DriveService driveService = new (client);

			var about = driveService.About;

			var response = about.Get();
			Assert.NotNull(response);

			Assert.IsInstanceOf<Google.Apis.Drive.v3.AboutResource.GetRequest>(
				response);
		}

		/// <summary>
		/// Service Account Environment Variable Success Test.
		/// </summary>
		[Test]

		public void ServiceAccountEnvironmentVariableSuccess()
		{
			string environmentVariable = "GOOGLE_APPLICATION_CREDENTIALS";
			Environment.SetEnvironmentVariable(
				environmentVariable, serviceAccountFilePath);

			BaseClientService.Initializer client =
				DigitalZenWorks.GoogleApiAuthorizer.Authorizer.Authorize(
					Mode.ServiceAccount,
					null,
					null,
					null,
					"Google Drive API File Uploader",
					scopes,
					null,
					false);

			Assert.NotNull(client);

			using DriveService driveService = new (client);

			var about = driveService.About;

			var response = about.Get();
			Assert.NotNull(response);

			Assert.IsInstanceOf<Google.Apis.Drive.v3.AboutResource.GetRequest>(
				response);
		}

		/// <summary>
		/// Service Account File Success Test.
		/// </summary>
		[Test]

		public void ServiceAccountFileSuccess()
		{
			BaseClientService.Initializer client =
				DigitalZenWorks.GoogleApiAuthorizer.Authorizer.Authorize(
					Mode.ServiceAccount,
					null,
					serviceAccountFilePath,
					null,
					"Google Drive API File Uploader",
					scopes,
					null,
					false);

			Assert.NotNull(client);

			using DriveService driveService = new (client);

			var about = driveService.About;

			var response = about.Get();
			Assert.NotNull(response);

			Assert.IsInstanceOf<Google.Apis.Drive.v3.AboutResource.GetRequest>(
				response);
		}

		/// <summary>
		/// Service Account No File Or Environement Variable Fail Test.
		/// </summary>
		[Test]

		public void ServiceAccountNoFileOrEnvironementVariableFail()
		{
			string environmentVariable = "GOOGLE_APPLICATION_CREDENTIALS";
			Environment.SetEnvironmentVariable(environmentVariable, null);

			BaseClientService.Initializer client =
				DigitalZenWorks.GoogleApiAuthorizer.Authorizer.Authorize(
					Mode.ServiceAccount,
					null,
					string.Empty,
					null,
					"Google Drive API File Uploader",
					scopes,
					null,
					false);

			Assert.Null(client);
		}

		/// <summary>
		/// Service Account Environment Variable Success Test.
		/// </summary>
		[Test]

		public void ServiceAccountObjectEnvironmentVariableSuccess()
		{
			Authorizer authorizer =
				new ("Google Drive API File Uploader", scopes, false);

			string environmentVariable = "GOOGLE_APPLICATION_CREDENTIALS";
			Environment.SetEnvironmentVariable(
				environmentVariable, serviceAccountFilePath);

			BaseClientService.Initializer client =
				authorizer.Authorize(
					Mode.ServiceAccount,
					null,
					string.Empty,
					null,
					null);

			Assert.NotNull(client);

			using DriveService driveService = new(client);

			var about = driveService.About;

			var response = about.Get();
			Assert.NotNull(response);

			Assert.IsInstanceOf<Google.Apis.Drive.v3.AboutResource.GetRequest>(
				response);
		}

		/// <summary>
		/// Service Account No File Or Environement Variable Fail Test.
		/// </summary>
		[Test]

		public void ServiceAccountObjectNoFileOrEnvironementVariableFail()
		{
			Authorizer authorizer =
				new ("Google Drive API File Uploader", scopes, false);

			string environmentVariable = "GOOGLE_APPLICATION_CREDENTIALS";
			Environment.SetEnvironmentVariable(environmentVariable, null);

			BaseClientService.Initializer client =
				authorizer.Authorize(
					Mode.ServiceAccount,
					null,
					string.Empty,
					null,
					null);

			Assert.Null(client);
		}

		/// <summary>
		/// Service Account No File Or Environement Variable Fail Test.
		/// </summary>
		[Test]

		public void
			ServiceAccountObjectDirectNoFileOrEnvironementVariableFail()
		{
			Authorizer authorizer =
				new("Google Drive API File Uploader", scopes, false);

			string environmentVariable = "GOOGLE_APPLICATION_CREDENTIALS";
			Environment.SetEnvironmentVariable(environmentVariable, null);

			BaseClientService.Initializer client =
				authorizer.AuthorizeServiceAccount(string.Empty);

			Assert.Null(client);
		}

		/// <summary>
		/// Tokens Direct No Credentials Fail Test.
		/// </summary>
		[Test]
		public void TokensDirectNoCredentialsFail()
		{
			BaseClientService.Initializer client = Authorizer.AuthorizeToken(
				null,
				tokensFilePath,
				"Google Drive API File Uploader",
				scopes);

			Assert.Null(client);
		}

		/// <summary>
		/// Tokens Direct No tokens Fail Test.
		/// </summary>
		[Test]
		public void TokensDirectNoTokensFail()
		{
			BaseClientService.Initializer client = Authorizer.AuthorizeToken(
				credentialsFilePath,
				string.Empty,
				"Google Drive API File Uploader",
				scopes);

			Assert.Null(client);
		}

		/// <summary>
		/// Tokens No Credentials Fail Test.
		/// </summary>
		[Test]
		public void TokensNoCredentialsFail()
		{
			BaseClientService.Initializer client = Authorizer.Authorize(
				Mode.Token,
				null,
				null,
				tokensFilePath,
				"Google Drive API File Uploader",
				scopes);

			Assert.Null(client);
		}

		/// <summary>
		/// Tokens No tokens Fail Test.
		/// </summary>
		[Test]
		public void TokensNoTokensFail()
		{
			BaseClientService.Initializer client = Authorizer.Authorize(
				Mode.Token,
				credentialsFilePath,
				null,
				string.Empty,
				"Google Drive API File Uploader",
				scopes,
				null,
				false);

			Assert.Null(client);
		}

		/// <summary>
		/// Tokens Object Direct No Credentials Fail Test.
		/// </summary>
		[Test]
		public void TokensObjectDirectNoCredentialsFail()
		{
			Authorizer authorizer =
				new ("Google Drive API File Uploader", scopes, false);

			BaseClientService.Initializer client = authorizer.AuthorizeToken(
				null,
				tokensFilePath);

			Assert.Null(client);
		}

		/// <summary>
		/// Tokens Object Direct No tokens Fail Test.
		/// </summary>
		[Test]
		public void TokensObjectDirectNoTokensFail()
		{
			Authorizer authorizer =
				new ("Google Drive API File Uploader", scopes, false);

			BaseClientService.Initializer client = authorizer.AuthorizeToken(
				credentialsFilePath,
				string.Empty);

			Assert.Null(client);
		}

		/// <summary>
		/// Tokens Object No Credentials Fail Test.
		/// </summary>
		[Test]
		public void TokensObjectNoCredentialsFail()
		{
			Authorizer authorizer =
				new ("Google Drive API File Uploader", scopes, false);

			BaseClientService.Initializer client = authorizer.Authorize(
				Mode.Token,
				null,
				null,
				tokensFilePath);

			Assert.Null(client);
		}

		/// <summary>
		/// Tokens Object No tokens Fail Test.
		/// </summary>
		[Test]
		public void TokensObjectNoTokensFail()
		{
			Authorizer authorizer =
				new ("Google Drive API File Uploader", scopes, false);

			BaseClientService.Initializer client = authorizer.Authorize(
				Mode.Token,
				credentialsFilePath,
				null,
				string.Empty);

			Assert.Null(client);
		}
	}
}
