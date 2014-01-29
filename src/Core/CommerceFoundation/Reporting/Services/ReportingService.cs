﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure;
using VirtoCommerce.Foundation.Assets.Model;
using VirtoCommerce.Foundation.Assets.Repositories;
using VirtoCommerce.Foundation.Frameworks;
using VirtoCommerce.Foundation.Reporting.Helpers;
using VirtoCommerce.Foundation.Reporting.Model;

namespace VirtoCommerce.Foundation.Reporting.Services
{
    [UnityInstanceProviderServiceBehaviorAttribute]
    public class ReportingService : IReportingService
    {
        public const string RootFolder = "reports";

        private readonly IBlobStorageProvider _blobStorageProvider;
        private readonly IAssetRepository _assetRepository;

        public ReportingService(IAssetRepository assetRepository, IBlobStorageProvider blobStorageProvider)
        {
            _blobStorageProvider = blobStorageProvider;
            _assetRepository = assetRepository;
            _assetRepository.CreateFolder(RootFolder);
        }

        public IEnumerable<ReportInfo> GetReportsList(string folderId = RootFolder)
        {
            if (string.IsNullOrWhiteSpace(folderId))
            {
                folderId = RootFolder;
            }

            List<ReportInfo> list = new List<ReportInfo>();
            GetReportItems(list, folderId);

            return list;
        }

        public IEnumerable<ReportFolder> GetReportsFolders()
        {
            List<ReportFolder> foldersList = new List<ReportFolder>();
            var folder = _assetRepository.GetFolderById(RootFolder);
            var root = new ReportFolder
            {
                FolderName = folder.Name.ToUpper(),
                FolderId = folder.FolderId,
                SubFoldersList = new List<ReportFolder>()
            }; 
            foldersList.Add(root);
            GetFolders(root.SubFoldersList as List<ReportFolder>, folder);

            return foldersList;
        }

        private void GetFolders(List<ReportFolder> list, Folder folder)
        {
            list.AddRange(
                _assetRepository.GetChildrenFolders(folder.FolderId)
                    .Select(delegate(Folder f)
                    {
                        var subFolder = new ReportFolder
                        {
                            FolderName = f.Name,
                            FolderId = f.FolderId,
                            SubFoldersList = new List<ReportFolder>()
                        };
                        GetFolders(subFolder.SubFoldersList as List<ReportFolder>, f);
                        return subFolder;
                    })
                    .ToList()
                );
        }

        private void GetReportItems(List<ReportInfo> list, string folderId)
        {
            list.AddRange(_assetRepository.GetChildrenFolderItems(folderId)
                //.Where(w=>Path.GetExtension(w.Name)==".rdl")
                .Select(f=>new ReportInfo
                {
                    Name = Path.GetFileNameWithoutExtension(f.Name),
                    AssetPath = f.FolderItemId
                }).ToList() );

            foreach (var childFolder in _assetRepository.GetChildrenFolders(folderId))
            {
                GetReportItems(list, childFolder.FolderId);
            }
        }

        public Stream GetReportFile(string reportFileName)
        {
            return _blobStorageProvider.OpenReadOnly(reportFileName);
        }

        public DataSet GetReportData(string reportFileName, IDictionary<string, object> parameters = null)
        {
            DataSet data = new DataSet();
            var report = RdlType.Load(GetReportFile(reportFileName));

            report.UpdateReportParameters(parameters);

            foreach(var dataSet in report.DataSets)
            {
                var connectionString = GetConnectionString(dataSet.Query.DataSourceName);
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    var firstOrDefault = report.DataSources.FirstOrDefault(d=>d.Name == dataSet.Query.DataSourceName);
                    if (firstOrDefault != null)
                    {
                        connectionString = firstOrDefault.ConnectionProperties.ConnectString;
                    }
                }

                using (var dbConnection = new SqlConnection(connectionString??string.Empty))
                {
                    dbConnection.Open();
                    var queryParameters = report.GetDataSetQueryParameters(dataSet.Name);
                    var table = ExecuteSQL(dbConnection, dataSet.Query.CommandText, queryParameters);
                    table.TableName = dataSet.Name;
                    data.Tables.Add(table);
                }
            }

            return data;
        }

        private DataTable ExecuteSQL(SqlConnection dbConn, string sqlCommand, IDictionary<string, object> parameters = null)
        {
            using (var cmd = dbConn.CreateCommand())
            {
                cmd.CommandText = string.Format(sqlCommand);

                if (parameters != null)
                {
                    foreach (var p in parameters)
                    {
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                    }
                }

                var adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
               
                return table;
            }
        }

        public string GetConnectionString(string nameOrConnectionString)
        {
            // try getting a settings first
            var settingValue = CloudConfigurationManager.GetSetting(nameOrConnectionString);

            if (String.IsNullOrEmpty(settingValue))
            {
                var connectionStringVal = ConfigurationManager.ConnectionStrings[nameOrConnectionString];

                settingValue = connectionStringVal == null ? string.Empty : connectionStringVal.ConnectionString;
            }

            return settingValue;
        }
    }
}
