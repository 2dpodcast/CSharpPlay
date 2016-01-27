using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LinqToTwitter;

namespace CSharpPlay.Authorization.ViewModel
{
	public class MainWindowViewModel : ViewModelBase
	{
		#region Property

		public string ConsumerKey
		{
			get { return _consumerKey; }
			set
			{
				Set(ref _consumerKey, value, true);
				OpenCommand.RaiseCanExecuteChanged();
			}
		}
		private string _consumerKey;

		public string ConsumerSecret
		{
			get { return _consumerSecret; }
			set
			{
				Set(ref _consumerSecret, value, true);
				OpenCommand.RaiseCanExecuteChanged();
			}
		}
		private string _consumerSecret;

		public string AccessToken
		{
			get { return _accessToken; }
			set { Set(ref _accessToken, value); }
		}
		private string _accessToken;

		public string AccessTokenSecret
		{
			get { return _accessTokenSecret; }
			set { Set(ref _accessTokenSecret, value); }
		}
		private string _accessTokenSecret;

		public string Pin
		{
			get { return _pin; }
			set
			{
				_pin = value;
				CompleteCommand.RaiseCanExecuteChanged();
			}
		}
		private string _pin;

		#endregion

		public RelayCommand OpenCommand { get; }
		public RelayCommand CompleteCommand { get; }

		public MainWindowViewModel()
		{
			OpenCommand = new RelayCommand(
				async () => await OpenAuthorizationPageAsync(),
				() => !string.IsNullOrWhiteSpace(ConsumerKey) && !string.IsNullOrWhiteSpace(ConsumerSecret));

			CompleteCommand = new RelayCommand(
				async () => await CompleteAuthorizationAsync(),
				() => (_authorizer != null) && !string.IsNullOrWhiteSpace(Pin));
		}

		private PinAuthorizer _authorizer;

		private async Task OpenAuthorizationPageAsync()
		{
			_authorizer = new PinAuthorizer
			{
				CredentialStore = new InMemoryCredentialStore
				{
					ConsumerKey = ConsumerKey,
					ConsumerSecret = ConsumerSecret
				},
				GoToTwitterAuthorization = pageLink => Process.Start(pageLink)
			};

			try
			{
				await _authorizer.BeginAuthorizeAsync();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private async Task CompleteAuthorizationAsync()
		{
			try
			{
				await _authorizer.CompleteAuthorizeAsync(Pin);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return;
			}

			AccessToken = _authorizer.CredentialStore.OAuthToken;
			AccessTokenSecret = _authorizer.CredentialStore.OAuthTokenSecret;

			SystemSounds.Asterisk.Play();
		}
	}
}