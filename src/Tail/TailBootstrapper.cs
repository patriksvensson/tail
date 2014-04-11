using Caliburn.Micro;
using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Extensions.Factory;
using Tail.ViewModels;
using Tail.Services;
using Tail.Extensibility;
using System.Reflection;
using System.Linq;
using BlackBox;

namespace Tail
{
	public class TailBootstrapper : BootstrapperBase
	{
		private readonly IKernel _kernel;
		private readonly ITailProviderScannerService _scanner;

		protected IKernel Kernel
		{
			get { return _kernel; }
		}

		public TailBootstrapper()
			: this(null)
		{
		}

		public TailBootstrapper(ITailProviderScannerService scanner)
			: base(true)
		{
			_kernel = new StandardKernel();
			_scanner = scanner ?? new TailProviderScannerService();

			// Start the Caliburn.Micro framework.
			Start();
		}

		protected override void Configure()
		{
			// Register Caliburn.Micro core stuff.
			_kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
			_kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();

			// Register services.
			_kernel.Bind<ITailListenerService>().To<TailListenerService>().InSingletonScope();
			_kernel.Bind<ITailProviderService>().To<TailProviderService>().InSingletonScope();
			_kernel.Bind<ITailSettingService>().To<TailSettingService>().InSingletonScope();

			// Register views models.
			_kernel.Bind<ShellViewModel>().To<ShellViewModel>().InSingletonScope();
			_kernel.Bind<StreamViewModel>().ToSelf(); // Factory scope
			_kernel.Bind<StreamConfigurationViewModel>().ToSelf(); // Factory scope
			_kernel.Bind<SettingsViewModel>().ToSelf(); // Factory scope

			// Register view model factories.
			_kernel.Bind<IStreamConfigurationViewModelFactory>().ToFactory();
			_kernel.Bind<IStreamViewModelFactory>().ToFactory();
			_kernel.Bind<ISettingsViewModelFactory>().ToFactory();

			// Register logging.
			RegisterLogging();

			// Register known providers.
			RegisterProviders();
		}

		private void RegisterLogging()
		{
			var configuration = LogConfiguration.FromConfigSection();
			if (configuration == null)
			{
				configuration = new LogConfiguration();
				configuration.Sinks.Add(new TraceSink());
			}

			// Create the kernel.
			var kernel = new LogKernel(configuration);
                       
			// Register the kernel and loggers in the container.
			_kernel.Bind<ILogKernel>().ToConstant(kernel).InSingletonScope();
			_kernel.Bind<ILogger>().ToMethod(x => x.Kernel.Get<ILogKernel>().GetLogger(x.Request.ParentRequest.Service));
		}

		private void RegisterProviders()
		{
			// Find all external providers.
			IEnumerable<Assembly> assemblies;
			var types = _scanner.Scan(out assemblies).ToArray();
			if (types.Length > 0)
			{
				foreach (var type in types)
				{
					// Bind the provider type.
					_kernel.Bind<ITailProvider>().To(type).InSingletonScope();
				}
				// Make sure Caliburn.Micro can find external views.
			    if (assemblies != null)
			    {
			        foreach (var assembly in assemblies)
			        {
			            AssemblySource.Instance.Add(assembly);
			        }
			    }
			}

			// Iterate all existing providers.
			foreach (var provider in _kernel.GetAll<ITailProvider>())
			{
				// Bind the listener.
				_kernel.Bind(provider.ListenerType).ToSelf();

				if (provider.ConfigurationType != null)
				{
					// Bind the configuration view model type.
					_kernel.Bind(provider.ConfigurationType).ToSelf();
				}
				if (provider.SettingsType != null)
				{
					// Bind the settings view model type.
					_kernel.Bind(provider.SettingsType).ToSelf();
				}
			}
		}

		protected override void BuildUp(object instance)
		{
			_kernel.Inject(instance);
		}

		protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
		{
			// Display the shell.
			DisplayRootViewFor<ShellViewModel>();
		}

		protected override void OnExit(object sender, EventArgs e)
		{
			if (!_kernel.IsDisposed)
			{
				// Dispose the kernel.
				_kernel.Dispose();
			}
			base.OnExit(sender, e);
		}

		protected override object GetInstance(Type service, string key)
		{
			if (service == null)
			{
				throw new ArgumentNullException("service");
			}
			return _kernel.Get(service);
		}

		protected override IEnumerable<object> GetAllInstances(Type service)
		{
			if (service == null)
			{
				throw new ArgumentNullException("service");
			}
			return _kernel.GetAll(service);
		}
	}
}
