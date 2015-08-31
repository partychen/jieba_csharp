using System;
using System.Configuration;
using log4net;
using log4net.Config;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace WordSegmenter.Test
{
	public class Bootstrapper
    {
		private const string LoggerName = "WordSegmenter";

		static Bootstrapper()
		{
			XmlConfigurator.Configure();
		}

		/// <summary>
		/// Configures the unity container.
		/// </summary>
		///
		/// <param name="container">
		/// The unity container.
		/// </param>
		public void Configure(IUnityContainer container)
		{
			try
			{
				container.RegisterType<ILog>(new InjectionFactory(factory => LogManager.GetLogger(LoggerName)));
				((UnityConfigurationSection)ConfigurationManager.GetSection("unity")).Configure(container);
			}
			catch (Exception ex)
			{
				LogManager.GetLogger(LoggerName).Error(ex);
				throw;
			}
		}
	}
}
