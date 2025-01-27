// <copyright file="GoogleApiAuthorizerTests.cs" company="Digital Zen Works">
// Copyright © 2022 - 2025 Digital Zen Works. All Rights Reserved.
// </copyright>

using Google.Apis.Drive.v3;
using Google.Apis.Services;
using NUnit.Framework;
using System;
using System.IO;

[assembly: CLSCompliant(true)]

namespace DigitalZenWorks.GoogleApiAuthorizer.Tests
{
	/// <summary>
	/// Test class for GoogleApiAuthorizer.
	/// </summary>
	public class GoogleApiAuthorizerTests
	{
		private static string testDataDirectory;
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
			testDataDirectory = Environment.GetEnvironmentVariable(
				"GOOGLEAPIAUTHORIZER_TEST_DIRECTORY");

			if (!string.IsNullOrWhiteSpace(testDataDirectory))
			{
				credentialsFilePath = testDataDirectory + @"\Credentials.json";
				serviceAccountFilePath =
					testDataDirectory + @"\ServiceAccount.json";
				tokensFilePath = testDataDirectory + @"\Tokens.json";
			}
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

			Assert.That(client, Is.Null);
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

			Assert.That(client, Is.Null);
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

			Assert.That(client, Is.Null);
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

			Assert.That(client, Is.Not.Null);

			using DriveService driveService = new (client);

			var about = driveService.About;

			var response = about.Get();

			Assert.That(response, Is.Not.Null);

			Assert.That(response, Is.InstanceOf<AboutResource.GetRequest>());
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

			Assert.That(client, Is.Not.Null);

			using DriveService driveService = new (client);

			var about = driveService.About;

			var response = about.Get();
			Assert.That(response, Is.Not.Null);

			Assert.That(response, Is.InstanceOf<AboutResource.GetRequest>());
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

			Assert.That(client, Is.Not.Null);

			using DriveService driveService = new (client);

			var about = driveService.About;

			var response = about.Get();
			Assert.That(response, Is.Not.Null);

			Assert.That(response, Is.InstanceOf<AboutResource.GetRequest>());
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

			Assert.That(client, Is.Not.Null);

			using DriveService driveService = new (client);

			var about = driveService.About;

			var response = about.Get();

			Assert.That(response, Is.Not.Null);

			Assert.That(response, Is.InstanceOf<AboutResource.GetRequest>());
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

			Assert.That(client, Is.Null);
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

			Assert.That(client, Is.Not.Null);

			using DriveService driveService = new(client);

			var about = driveService.About;

			var response = about.Get();

			Assert.That(response, Is.Not.Null);

			Assert.That(response, Is.InstanceOf<AboutResource.GetRequest>());
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

			Assert.That(client, Is.Null);
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

			Assert.That(client, Is.Null);
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

			Assert.That(client, Is.Null);
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

			Assert.That(client, Is.Null);
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

			Assert.That(client, Is.Null);
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

			Assert.That(client, Is.Null);
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

			Assert.That(client, Is.Null);
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

			Assert.That(client, Is.Null);
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

			Assert.That(client, Is.Null);
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

			Assert.That(client, Is.Null);
		}
	}
}
