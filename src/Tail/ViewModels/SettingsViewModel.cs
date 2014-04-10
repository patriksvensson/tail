using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using Tail.Extensibility;
using Tail.Models;

namespace Tail.ViewModels
{
	public interface ISettingsViewModelFactory
	{
        ////////////////////////////////////////////////
        // This is a factory interface used by Ninject.
        // Do not remove this even if it seem unused.
        ////////////////////////////////////////////////

		SettingsViewModel Create();
	}

	public class SettingsViewModel : Conductor<ITailSettings>.Collection.OneActive
	{
		private readonly ITailSettingService _settingsService;
		private readonly Dictionary<Type, ITailSettings> _viewModels;
		private readonly List<TailProviderInfo> _providers;
		private TailProviderInfo _selectedProvider;
		private string _validationMessage;

		public List<TailProviderInfo> Providers
		{
			get { return _providers; }
		}

		public TailProviderInfo SelectedProvider
		{
			get { return _selectedProvider; }
			set
			{
				_selectedProvider = value;
				NotifyOfPropertyChange(() => SelectedProvider);
			}
		}

		public string ValidationMessage
		{
			get { return _validationMessage; }
			set
			{
				_validationMessage = value;
				NotifyOfPropertyChange(() => ValidationMessage);
			}
		}

		public bool CanSave
		{
			get { return _providers.Count > 1; }
		}

		public SettingsViewModel(ITailProviderService service, ITailSettingService settingsService, GeneralSettingsViewModel generalSettings)
		{
			_settingsService = settingsService;
			_viewModels = new Dictionary<Type, ITailSettings>();
			_providers = new List<TailProviderInfo>();

			var types = service.GetProviderTypes();
			foreach (var type in types)
			{
				var viewModel = service.CreateSettings(type);
				if (viewModel != null)
				{
					var displayName = service.GetDisplayName(type);
					if (!string.IsNullOrWhiteSpace(displayName))
					{
						// Load the settings for the view model.
						viewModel.Load(_settingsService);

						// Add the settings to the collection.
						_viewModels.Add(viewModel.GetType(), viewModel);
						_providers.Add(new TailProviderInfo(displayName, viewModel.GetType()));
					}
				}
			}

			// Sort all providers by name.
			_providers = _providers.OrderBy(x => x.Name).ToList();

			// Insert the general settings.
			_viewModels.Add(typeof(GeneralSettingsViewModel), generalSettings);
			_providers.Insert(0, new TailProviderInfo("General", typeof(GeneralSettingsViewModel)));

			// Select the first provider in the list.
			if (_providers.Count > 0)
			{
				SelectedProvider = Providers[0];
				ToggleProvider(Providers[0]);
			}

			DisplayName = "Settings";
		}

		public void ToggleProvider(TailProviderInfo eventArgs)
		{
			if (_viewModels.ContainsKey(eventArgs.Type))
			{
				// Set the active item.
				ActivateItem(_viewModels[eventArgs.Type]);
			}
			else
			{
				// Close the currently active item.
				this.CloseItem(ActiveItem);
			}
		}

		public void Save()
		{
			foreach (var provider in _providers)
			{
				var viewModel = _viewModels[provider.Type];
				if (viewModel.IsDirty)
				{
					string error;
					if (!viewModel.Validate(out error))
					{
						// Select the provider.
						SelectedProvider = provider;
						ActivateItem(viewModel);

						// Set the validation error message.
						ValidationMessage = error;

						return;
					}

					// Save the view model.
					viewModel.Save(_settingsService);
				}
			}

			// Close the dialog.
			TryClose(true);
		}
	}
}
