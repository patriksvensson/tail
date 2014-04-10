using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using Tail.Extensibility;
using Tail.Messages;
using Tail.Models;

namespace Tail.ViewModels
{
	public interface IStreamConfigurationViewModelFactory
	{
        ////////////////////////////////////////////////
        // This is a factory interface used by Ninject.
        // Do not remove this even if it seem unused.
        ////////////////////////////////////////////////

		StreamConfigurationViewModel Create();
	}

	public class StreamConfigurationViewModel : Conductor<ITailConfiguration>.Collection.OneActive
	{
		private readonly ITailProviderService _providerService;
		private readonly IEventAggregator _eventAggregator;
		private readonly List<TailProviderInfo> _providers;
		private readonly Dictionary<Type, ITailConfiguration> _viewModels;
		private TailProviderInfo _selectedProvider;

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

		public StreamConfigurationViewModel(ITailProviderService providerService, IEventAggregator eventAggregator)
		{
			_providerService = providerService;
			_eventAggregator = eventAggregator;
			_viewModels = new Dictionary<Type, ITailConfiguration>();

			// Create the provider information.
			var providers = new List<TailProviderInfo>();
			foreach (var type in providerService.GetProviderTypes())
			{
				var name = _providerService.GetDisplayName(type);
				if (string.IsNullOrWhiteSpace(name))
				{
					continue;
				}

				var viewModel = _providerService.CreateConfigurationViewModel(type);
				if (viewModel != null)
				{
					_viewModels.Add(type, viewModel);
				}

				providers.Add(new TailProviderInfo(name, type));
			}

			// Sort all providers by name.
			_providers = new List<TailProviderInfo>(providers.OrderBy(p => p.Name));

			// Select the first provider in the list.
			if (_providers.Count > 0)
			{
				SelectedProvider = Providers[0];
				ToggleProvider(Providers[0]);
			}

			// Set the display name.
			DisplayName = "Open Stream";
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

		public void OpenStream()
		{
			// Get the currently selected provider.
			var selectedProvider = _selectedProvider;
			if (selectedProvider != null)
			{
				// Find the view model.
				var type = selectedProvider.Type;

				if (IsValidConfiguration(type))
				{
					var listener = _providerService.CreateListener(type);
					var context = CreateContext(type);

					if (listener != null && context != null)
					{
						// Send a request to start listening.
						_eventAggregator.Publish(new StartListeningEvent(listener, context));

						// Close the window.
						TryClose(true);
					}
				}
			}
		}

		private bool IsValidConfiguration(Type type)
		{
			if (_viewModels.ContainsKey(type))
			{
				var viewModel = _viewModels[type];
				return viewModel.Validate();
			}
			return true;
		}

		private ITailStreamContext CreateContext(Type type)
		{
			if (_viewModels.ContainsKey(type))
			{
				var viewModel = _viewModels[type];
				return _providerService.CreateContext(type, viewModel);
			}
			return _providerService.CreateContext(type, null);
		}
	}
}
