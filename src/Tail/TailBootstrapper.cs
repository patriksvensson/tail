// ﻿
// Copyright (c) 2013 Patrik Svensson
// 
// This file is part of Tail.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Extensions.Factory;
using Tail.ViewModels;
using Tail.Services;
using Tail.Extensibility;
using Tail.Providers;
using System.IO;
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
			this.Start();
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
			this.RegisterLogging();

			// Register known providers.
			this.RegisterProviders();
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
			var assemblies = (IEnumerable<Assembly>)null;
			var types = _scanner.Scan(out assemblies).ToArray();
			if (types.Length > 0)
			{
				foreach (var type in types)
				{
					// Bind the provider type.
					_kernel.Bind<ITailProvider>().To(type).InSingletonScope();
				}
				// Make sure Caliburn.Micro can find external views.
				foreach (var assembly in assemblies)
				{
					AssemblySource.Instance.Add(assembly);
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
			this.DisplayRootViewFor<ShellViewModel>();
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
