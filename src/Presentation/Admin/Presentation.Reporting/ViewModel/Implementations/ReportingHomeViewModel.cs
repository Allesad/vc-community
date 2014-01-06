﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using VirtoCommerce.Foundation.Reporting.Services;
using VirtoCommerce.ManagementClient.Core.Infrastructure;
using VirtoCommerce.ManagementClient.Core.Infrastructure.DataVirtualization;
using VirtoCommerce.ManagementClient.Core.Infrastructure.Navigation;
using VirtoCommerce.ManagementClient.Reporting.ViewModel.Interfaces;
using VirtoCommerce.Foundation.Reporting.Model;
using System.Threading;

namespace VirtoCommerce.ManagementClient.Reporting.ViewModel.Implementations
{
    public class ReportingHomeViewModel : ViewModelHomeEditableBase<ReportInfo>, IReportingHomeViewModel, IVirtualListLoader<IReportViewModel>, ISupportDelayInitialization
    {
        public IList<TreeViewItem> ReportItemsTree { get; private set; }
        public DelegateCommand<object> GenerateReportCommand { get; private set; }
        public DelegateCommand<object> TreeViewSelectedItemChangedCommand { get; private set; }
        public InteractionRequest<Notification> CommonNotifyRequest { get; private set; }

        private readonly IReportingService _reportService;
        private readonly IViewModelsFactory<IReportViewModel> _itemVmFactory;
        private readonly IAuthenticationContext _authContext;
        private readonly INavigationManager _navManager;

        public ReportingHomeViewModel(
            IReportingService reportService, 
            IViewModelsFactory<IReportViewModel> itemVmFactory,
            IAuthenticationContext authContext,
            INavigationManager navManager)
        {
            _reportService = reportService;
            _itemVmFactory = itemVmFactory;
            _navManager = navManager;
            _authContext = authContext;

            ViewTitle = new ViewTitleBase()
            {
                Title = "Reports",
                SubTitle = ""
            };
            
            CommandsInits();
        }

        public void InitializeForOpen()
        {
            OnUIThread(delegate()
            {
                ListItemsSource = new VirtualList<IReportViewModel>(this, SynchronizationContext.Current);
                _reportFolder = _reportService.GetReportsFolders();
            });
        }

        private IEnumerable<ReportFolder> _reportFolder ;
        private IViewModel _selectedReportFolderItem;

        public IEnumerable<ReportFolder> ReportFolder
        {
            get { return _reportFolder; }
        }

        public object SelectedTreeItem { get; private set; }

        private void CommandsInits()
        {
            RefreshItemsCommand = new DelegateCommand(RaiseRefreshCommand);
            GenerateReportCommand = new DelegateCommand<object>(RaiseGenerateCommand);
            TreeViewSelectedItemChangedCommand = new DelegateCommand<object>(TreeViewSelectedItemChanged);
            CommonNotifyRequest = new InteractionRequest<Notification>();
        }

        private void RaiseGenerateCommand(object item)
        {
            if (item == null)
            {
                CommonNotifyRequest.Raise(new Notification {Content = "Select report to generate.", Title = "Error"});
            }
            else
            {
                var reportItem = ((VirtualListItem<IReportViewModel>)item).Data.InnerItem;
                var itemVM = _itemVmFactory.GetViewModelInstance(
                    new KeyValuePair<string, object>("item", reportItem)
                    );
                var confirmation = new Confirmation { Content = itemVM, Title = "Generate report" };
               CommonWizardDialogRequest.Raise(confirmation);
            }
        }


        private void TreeViewSelectedItemChanged(object item)
        {
            if (SelectedTreeItem != item)
            {
                SelectedTreeItem = item;
                ListItemsSource.Refresh();
            }
        }

        protected override void RaiseRefreshCommand()
        {
            ListItemsSource.Refresh();
        }

        protected override bool CanItemAddExecute()
        {
            return true;
        }

        protected override bool CanItemDeleteExecute(IList x)
        {
            return x != null && x.Count > 0;
        }

        protected override void RaiseItemAddInteractionRequest()
        {
            
        }

        protected override void RaiseItemDeleteInteractionRequest(IList selectedItemsList)
        {

        }

        public bool CanSort {
            get { return true; }
        }

        public IList<IReportViewModel> LoadRange(int startIndex, int count, SortDescriptionCollection sortDescriptions, out int overallCount)
        {
            var selectedFolder = SelectedTreeItem as ReportFolder;
            var reportFolder = string.Empty;

            if (selectedFolder != null)
            {
                reportFolder = selectedFolder.FullPath;
            }

            var retVal = new List<IReportViewModel>();
            var list = _reportService.GetReportsList(reportFolder);

            overallCount = list.Count();
            var items = list.Take(count).ToList();
            retVal.AddRange(items.Select(item => _itemVmFactory.GetViewModelInstance(new KeyValuePair<string, object>("item", item))));

            return retVal;
        }
    }
}
