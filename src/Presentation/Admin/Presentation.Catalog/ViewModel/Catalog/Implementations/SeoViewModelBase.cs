﻿#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using VirtoCommerce.Foundation.AppConfig.Factories;
using VirtoCommerce.ManagementClient.Catalog.Model;
using ObjectModel = VirtoCommerce.Foundation.AppConfig.Model;
using VirtoCommerce.Foundation.AppConfig.Repositories;
using VirtoCommerce.Foundation.Frameworks;
using Omu.ValueInjecter;
using VirtoCommerce.ManagementClient.Catalog.ViewModel.Catalog.Interfaces;
using VirtoCommerce.ManagementClient.Core.Infrastructure;

#endregion

namespace VirtoCommerce.ManagementClient.Catalog.ViewModel.Catalog.Implementations
{
	public abstract class SeoViewModelBase : ViewModelBase, ISeoViewModel
	{
		#region Dependencies

		protected readonly IRepositoryFactory<IAppConfigRepository> _appConfigRepositoryFactory;
		private readonly IAppConfigEntityFactory _appConfigEntityFactory;
		private readonly string _defaultLanguage;
		private readonly string _keywordValue;
		private readonly ObjectModel.SeoUrlKeywordTypes _keywordType;
		private readonly List<string> _availableLanguages;
		
		#endregion

		#region Constructor
		
		protected SeoViewModelBase(
			IRepositoryFactory<IAppConfigRepository> appConfigRepositoryFactory, 
			IAppConfigEntityFactory appConfigEntityFactory, 
			string defaultLanguage, 
			IEnumerable<string> languages,
			string keywordValue,
			ObjectModel.SeoUrlKeywordTypes keywordType)
		{
			_appConfigRepositoryFactory = appConfigRepositoryFactory;
			_appConfigEntityFactory = appConfigEntityFactory;
			_availableLanguages = languages.ToList();
			_defaultLanguage = defaultLanguage;
			_keywordValue = keywordValue;
			_keywordType = keywordType;

			InitCommands();
		}

		#endregion
		
		#region Public properties

		public string NavigateUrl
		{
			get
			{
				return string.IsNullOrEmpty(CurrentSeoKeyword.BaseUrl)
					                ? string.Empty
									: string.Format("{0}/{1}", CurrentSeoKeyword.BaseUrl, string.IsNullOrEmpty(CurrentSeoKeyword.Keyword) ? string.IsNullOrEmpty(SeoKeywords.First(x => x.Language.Equals(_defaultLanguage, StringComparison.OrdinalIgnoreCase)).Keyword) ? CurrentSeoKeyword.KeywordValue : SeoKeywords.First(x => x.Language.Equals(_defaultLanguage, StringComparison.OrdinalIgnoreCase)).Keyword : CurrentSeoKeyword.Keyword);
			}
		}

		public bool IsValid
		{
			get
			{
				var retVal = SeoKeywords.All(keyword => keyword.Validate()) && ValidateKeywords();
				if (!retVal)
				{
					SeoLocalesFilterCommand.Execute(SeoKeywords.First(x => x.Errors.Any()));
				}

				return retVal;
			}
		}

		public string Description
		{
			get
			{
				return "Enter item SEO information.";
			}
		}

		private SeoUrlKeyword _currentSeoKeyword;
		public SeoUrlKeyword CurrentSeoKeyword
		{
			get { return _currentSeoKeyword; }
			set { _currentSeoKeyword = value; OnPropertyChanged(); }
		}

		#endregion

		#region ISeoViewModel

		public List<SeoUrlKeyword> SeoKeywords { get; protected set; }

		public void UpdateKeywordValueCode(string newCode)
		{
			SeoKeywords.ForEach(x =>
				{
					x.KeywordValue = newCode;
					x.IsModified = true;
				});
		}

		public virtual void UpdateSeoKeywords()
		{
			//if any SEO keyword modified update or add it
			if (SeoKeywords.Any(x => x.IsModified))
			{
				using (var appConfigRepository = _appConfigRepositoryFactory.GetRepositoryInstance())
				{
					SeoKeywords.Where(x => x.IsModified && x.Validate()).ToList().ForEach(keyword =>
					{
						if (string.IsNullOrEmpty(keyword.Keyword))
						{
							var keywordToRemove =
								appConfigRepository.SeoUrlKeywords.Where(
									seoKeyword => true && seoKeyword.SeoUrlKeywordId.Equals(keyword.SeoUrlKeywordId)).FirstOrDefault();
							if (keywordToRemove != null)
							{
								keywordToRemove.KeywordValue = keyword.KeywordValue;
								keywordToRemove.IsActive = false;
								appConfigRepository.Update(keywordToRemove);
							}
						}
						else
						{
							var originalKeyword =
								appConfigRepository.SeoUrlKeywords.Where(
									seoKeyword => true &&
									              seoKeyword.SeoUrlKeywordId.Equals(keyword.SeoUrlKeywordId))
								                   .FirstOrDefault();

							if (originalKeyword != null)
							{
								originalKeyword.Title = keyword.Title;
								originalKeyword.MetaDescription = keyword.MetaDescription;
								originalKeyword.Keyword = keyword.Keyword;
								originalKeyword.ImageAltDescription = keyword.ImageAltDescription;
								originalKeyword.KeywordValue = keyword.KeywordValue;
								appConfigRepository.Update(originalKeyword);
							}
							else
							{
								var addKeyword = _appConfigEntityFactory.CreateEntity<ObjectModel.SeoUrlKeyword>();
								addKeyword.IsActive = true;
								addKeyword.InjectFrom(keyword);
								appConfigRepository.Add(addKeyword);
							}
						}
					});
					appConfigRepository.UnitOfWork.Commit();
					CurrentSeoKeyword.PropertyChanged -= CurrentSeoKeyword_PropertyChanged;
					SeoKeywords.ForEach(y => y.IsModified = false);
					CurrentSeoKeyword.PropertyChanged += CurrentSeoKeyword_PropertyChanged;
					OnPropertyChanged("NavigateUrl");
				}
			}
		}

		#endregion

		#region Use custom properties

		private bool _useCustomMetaDescription;
		public bool UseCustomMetaDescription
		{
			get { return _useCustomMetaDescription || !string.IsNullOrEmpty(CurrentSeoKeyword.MetaDescription); }
			set
			{
				_useCustomMetaDescription = value;
				if (!_useCustomMetaDescription && !string.IsNullOrEmpty(CurrentSeoKeyword.MetaDescription))
					CurrentSeoKeyword.MetaDescription = null;
				OnPropertyChanged();
			}
		}

		private bool _useCustomTitle;
		public bool UseCustomTitle
		{
			get { return _useCustomTitle || !string.IsNullOrEmpty(CurrentSeoKeyword.Title); }
			set
			{
				_useCustomTitle = value;
				if (!_useCustomTitle && !string.IsNullOrEmpty(CurrentSeoKeyword.Title))				
					CurrentSeoKeyword.Title = null;
				OnPropertyChanged();				
			}
		}

		private bool _useCustomImageText;
		public bool UseCustomImageText
		{
			get { return _useCustomImageText || !string.IsNullOrEmpty(CurrentSeoKeyword.ImageAltDescription); }
			set
			{
				_useCustomImageText = value;
				if (!_useCustomImageText && !string.IsNullOrEmpty(CurrentSeoKeyword.ImageAltDescription))
					CurrentSeoKeyword.ImageAltDescription = null;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Commands

		public DelegateCommand<SeoUrlKeyword> SeoLocalesFilterCommand { get; private set; }
		public DelegateCommand NavigateToUrlCommand { get; private set; }

		#endregion

		#region virtual and abstract methods
		
		protected virtual void InitializePropertiesForViewing()
		{
			SeoKeywords = new List<SeoUrlKeyword>();
			using (var _appConfigRepository = _appConfigRepositoryFactory.GetRepositoryInstance())
			{
				_appConfigRepository.SeoUrlKeywords.Where(
					keyword =>
					keyword.KeywordValue.Equals(_keywordValue) && keyword.KeywordType.Equals((int) _keywordType) && keyword.IsActive)
									.ToList().ForEach(seo =>
										{
											var newSeo = new SeoUrlKeyword(seo);
											newSeo.BaseUrl = BuildBaseUrl(newSeo);
											SeoKeywords.Add(newSeo);
										});
			}

			_availableLanguages.ForEach(locale => 
				{
					if (!SeoKeywords.Any(keyword => keyword.Language.ToLowerInvariant().Equals(locale.ToLowerInvariant())))
					{
						var newSeoKeyword = new SeoUrlKeyword { Language = locale, KeywordType = (int)_keywordType, KeywordValue = _keywordValue };
						newSeoKeyword.BaseUrl = BuildBaseUrl(newSeoKeyword);
						SeoKeywords.Add(newSeoKeyword);
					}
				});
			
			SeoLocalesFilterCommand.Execute(SeoKeywords.First(x => x.Language.ToLowerInvariant().Equals(_defaultLanguage.ToLowerInvariant())));
		}

		protected abstract string BuildBaseUrl(SeoUrlKeyword keyword);

		#endregion

		#region Auxilliary methods

		private void InitCommands()
		{
			SeoLocalesFilterCommand = new DelegateCommand<SeoUrlKeyword>(RaiseSeoLocaleChange);
			NavigateToUrlCommand = new DelegateCommand(RaiseNavigateToUrl);
		}

		private void RaiseSeoLocaleChange(SeoUrlKeyword currentKeyword)
		{
			if (CurrentSeoKeyword != null)
				CurrentSeoKeyword.PropertyChanged -= CurrentSeoKeyword_PropertyChanged;
			
			CurrentSeoKeyword = currentKeyword;
			
			if (CurrentSeoKeyword != null)
				CurrentSeoKeyword.PropertyChanged += CurrentSeoKeyword_PropertyChanged;

			ResetCustomProperties();
		}

		void CurrentSeoKeyword_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			CurrentSeoKeyword.IsModified = true;
			CurrentSeoKeyword.Validate();
			OnPropertyChanged("CurrentSeoKeyword");
		}

		private void ResetCustomProperties()
		{
			_useCustomImageText = !string.IsNullOrEmpty(CurrentSeoKeyword.ImageAltDescription);
			_useCustomMetaDescription = !string.IsNullOrEmpty(CurrentSeoKeyword.MetaDescription);
			_useCustomTitle = !string.IsNullOrEmpty(CurrentSeoKeyword.Title);
			OnPropertyChanged("UseCustomTitle");
			OnPropertyChanged("UseCustomMetaDescription");
			OnPropertyChanged("UseCustomImageText");
			OnPropertyChanged("NavigateUrl");
		}

		private void RaiseNavigateToUrl()
		{
			System.Diagnostics.Process.Start(NavigateUrl);
		}

		private bool ValidateKeywords()
		{
			var retVal = true;
			var keywords = SeoKeywords.Where(key => !string.IsNullOrEmpty(key.Keyword)).ToList();
			if (keywords.Any())
			{
				using (var appConfigRepository = _appConfigRepositoryFactory.GetRepositoryInstance())
				{
					keywords.ForEach(keyword =>
					{
						if (retVal)
						{
							var count = appConfigRepository.SeoUrlKeywords
														   .Where(x =>
																  x.SeoUrlKeywordId != keyword.SeoUrlKeywordId &&
																  x.Keyword == keyword.Keyword && x.KeywordType == keyword.KeywordType && x.Language == keyword.Language && x.IsActive)
														   .Count();

							if (count > 0)
							{
								keyword.SetError("Keyword", "Item with the same Keyword and Language already exists", false);
								if (keyword.SeoUrlKeywordId.Equals(CurrentSeoKeyword.SeoUrlKeywordId))
									OnPropertyChanged("CurrentSeoKeyword");
								retVal = false;
							}
						}
					});
				}
			}

			return retVal;
		}

		#endregion
	}
}
