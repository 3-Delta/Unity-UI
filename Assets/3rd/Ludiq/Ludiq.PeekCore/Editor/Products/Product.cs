using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class Product : IAboutable
	{
		protected Product(string id)
		{
			Ensure.That(nameof(id)).IsNotNull(id);

			this.id = id;

			_plugins = new List<Plugin>();
			plugins = _plugins.AsReadOnly();
		}

		public virtual void Initialize()
		{

		}

		internal readonly List<Plugin> _plugins;

		public string id { get; }

		public ReadOnlyCollection<Plugin> plugins { get; }

		public virtual bool requiresSetup => plugins.Any(p => !p.configuration.projectSetupCompleted /*|| !p.configuration.editorSetupCompleted*/);

		public abstract string name { get; }
		public abstract string author { get; }
		public abstract string description { get; }
		public Texture2D logo { get; protected set; }
		public abstract SemanticVersion version { get; }

		public virtual string authorLabel => "Author: ";
		public Texture2D authorLogo { get; protected set; }
		public virtual string copyrightHolder => author;
		public virtual int copyrightYear => DateTime.Now.Year;

		public virtual string publisherUrl => null;
		public virtual string websiteUrl => null;
		public string authorUrl => publisherUrl;
		public string url => websiteUrl;
		
		protected virtual string rootFileName => id + ".root";

		private string _rootPath;

		public string rootPath
		{
			get
			{
				if (_rootPath == null)
				{
					_rootPath = PathUtility.GetRootPath(rootFileName, $"Ludiq/{id}/{rootFileName}", true);
				}

				return _rootPath;
			}
		}

		public virtual IEnumerable<Page> SetupWizardIntroductionPages(SetupWizard wizard)
		{
			yield return new WelcomePage(this, wizard);
		}

		public virtual IEnumerable<Page> SetupWizardConclusionPages(SetupWizard wizard)
		{
			yield return new SetupCompletePage(this, wizard);
		}
	}
}