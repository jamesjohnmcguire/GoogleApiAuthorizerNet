// <copyright file="GoogleApiAuthorizerTests.cs" company="Digital Zen Works">
// Copyright © 2022 Digital Zen Works. All Rights Reserved.
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
		private string serviceAccountFilePath;

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
			serviceAccountFilePath = dataPath + @"\ServiceAccount.json";
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
		public void TestSanityCheck()
		{
			Assert.Pass();
		}

		/// <summary>
		/// Service Account Direct No File Or Environement Variable Fail Test.
		/// </summary>
		[Test]

		public void TestServiceAccountDirectNoFileOrEnvironementVariableFail()
		{
			string environmentVariable = "GOOGLE_APPLICATION_CREDENTIALS";
			Environment.SetEnvironmentVariable(environmentVariable, null);

			string[] scopes = { "https://www.googleapis.com/auth/drive" };

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

		public void TestServiceAccountDirectEnvironmentVariableSuccess()
		{
			string environmentVariable = "GOOGLE_APPLICATION_CREDENTIALS";
			Environment.SetEnvironmentVariable(
				environmentVariable, serviceAccountFilePath);

			string[] scopes = { "https://www.googleapis.com/auth/drive" };

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

		public void TestServiceAccountDirectFileSuccess()
		{
			string[] scopes = { "https://www.googleapis.com/auth/drive" };

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

		public void TestServiceAccountEnvironmentVariableSuccess()
		{
			string environmentVariable = "GOOGLE_APPLICATION_CREDENTIALS";
			Environment.SetEnvironmentVariable(
				environmentVariable, serviceAccountFilePath);

			string[] scopes = { "https://www.googleapis.com/auth/drive" };

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

			using DriveService driveService = new(client);

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

		public void TestServiceAccountFileSuccess()
		{
			string[] scopes = { "https://www.googleapis.com/auth/drive" };

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

		public void TestServiceAccountNoFileOrEnvironementVariableFail()
		{
			string environmentVariable = "GOOGLE_APPLICATION_CREDENTIALS";
			Environment.SetEnvironmentVariable(environmentVariable, null);

			string[] scopes = { "https://www.googleapis.com/auth/drive" };

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
	}
}
