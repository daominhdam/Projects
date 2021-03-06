﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using System.Reflection;
using AutomationTestAssistantCore;
using AutomationTestAssistantCore.ExecutionEngine.Messages;

namespace ServerClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient clientTcpWriter;
        private TcpClient clientTcpListner;
        private TcpListener serverMsBuildUpdaterListener;
        private TcpClient clientMsBuildTcpAgentListner;
        //private static TcpClient msBuildClientTcpWriter;
        //private static Thread msBuildLogListnerThreadWorker;
        private delegate void UpdateProgress(string currentProgress);
        private bool isConnected;
        private List<string> testIds;
        private string relativeMsTestProjPath = "\\..\\..\\..\\..\\MsTest.proj";
        private string relativeTfsProjPath = "\\..\\..\\..\\..\\TfsOperations.proj";
        private string msTestProjFullPath;
        private string tfsProjFullPath;
        private IpAddressSettings clientSettings;
        private IpAddressSettings agentSettings;
        private IpAddressSettings msBuildLogSettings;

        public MainWindow()
        {
            InitializeComponent();
            testIds = new List<string>();
            BuildMsBuildProjectsFullPaths();
            GetIpsInformation();
            List<ProjectInfo> projectPaths = new List<ProjectInfo>()
            {
               new ProjectInfo( @"D:\AutomationTestAssistant\TestTime\TestTime.csproj")
            };
            dgvProjects.ItemsSource = projectPaths;

            List<Object> workspaceInfos = new List<Object>();
            var tfsInfo = new
                {
                    TfsPath = "tfsTestPath",
                    LocalPath = "localPathTest",
                    TfsProjectCollection = "CorporateSites",
                    WorkspaceName = "TestWorkspaceName"
                };
            workspaceInfos.Add(tfsInfo);
            var tfsInfo1 = new
            {
                TfsPath = "tfsTestPath1",
                LocalPath = "localPathTest1",
                TfsProjectCollection = "CorporateSites1",
                WorkspaceName = "TestWorkspaceName1"
            }; 
            workspaceInfos.Add(tfsInfo1);
            dgvTfsSettings.ItemsSource = workspaceInfos;
            //dgvProjects.Rows[0].Cells[0].Value = @"D:\AutomationTestAssistant\TestTime\TestTime.csproj";
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            //List<string> projectPaths = GetProjectPaths();
            //List<string> assembliesPaths = ATACore.Project.ProjectInfoCollector.GetAssemblyPathsByProjectPaths(projectPaths);
            //List<TestLink> testLinks = ATACore.TestExecution.TestListGenerator.GetProjectTestLinksMultipleAssemblies(assembliesPaths);
            //MethodInfo[] methodInfos = ATACore.Project.ProjectInfoCollector.GetProjectTestMethodsMultipleAssemblies(assembliesPaths);
            //List<string> testIdsToBeExecuted = FilterCheckedTests();
            //testLinks = ATACore.TestExecution.TestListGenerator.RemoveTestLinksNotSpecifiedIds(testIdsToBeExecuted, testLinks);
            //string testListContent = ATACore.TestExecution.TestListGenerator.GenerateListOfSpecifiedTests(testLinks);

            //string uniqueTestResultName = ATACore.Utilities.TimeStampGenerator.GenerateTrxFilePath();
            ////HACK NEED TO BE REWRITTEN, Test Links STORAGE Should be mapped to the get latest on the agent
            //string messageToBeSend = ATACore.CommandExecutor.GenerateMsTestMessage(msTestProjFullPath.Replace("D", "C"), testListContent.Replace("D:\\", "C:\\"), uniqueTestResultName, tbTestListName.Text, msBuildLogSettings);

            //ATACore.TcpWrapperProcessor.TcpClientWrapper.SendMessageToClient(clientTcpWriter, messageToBeSend);
        }

        #region Private Logic Methods

        private void GetIpsInformation()
        {
            agentSettings = new IpAddressSettings(tbAgentIp.Text);
            clientSettings = new IpAddressSettings(tbClientIp.Text);
            msBuildLogSettings = new IpAddressSettings(tbMsBuildLogIp.Text);
        }

        private void TestGetIpsInformation()
        {
            agentSettings = new IpAddressSettings("127.0.0.1:8887");
            clientSettings = new IpAddressSettings("127.0.0.1:8888");
            msBuildLogSettings = new IpAddressSettings("127.0.0.1:8889");
        }

        private void BuildMsBuildProjectsFullPaths()
        {
            string currentExecutablePath = Assembly.GetExecutingAssembly().Location;

            string asseblyFullPath = String.Concat(currentExecutablePath, relativeMsTestProjPath);
            msTestProjFullPath = System.IO.Path.GetFullPath(asseblyFullPath);
            asseblyFullPath = String.Concat(currentExecutablePath, relativeTfsProjPath);
            tfsProjFullPath = System.IO.Path.GetFullPath(asseblyFullPath);
        }

        private void AddMethodsToCheckBoxListInternal(string currentProjectDllPath, MethodInfo[] currentDllMethods)
        {
            int i = 0;
            //foreach (MethodInfo currentMethodInfo in currentDllMethods)
            //{
            //    dgvMethods.Rows.Add(new DataGridViewRow());
            //    dgvMethods.Rows[i].Cells[1].Value = currentMethodInfo.Name;
            //    dgvMethods.Rows[i].Cells[2].Value = currentMethodInfo.DeclaringType.FullName;
            //    dgvMethods.Rows[i].Cells[3].Value = ATACore.TestExecution.TestListGenerator.GenerateTestMethodId(currentMethodInfo, currentProjectDllPath);
            //    i++;
            //}
        }

        private void RefreshInternal()
        {
            for (int i = 0; i < dgvProjects.Items.Count; i++)
            {
                string currentProjectPath = String.Empty;
                TextBlock x = dgvProjects.Columns[0].GetCellContent(dgvProjects.Items[i]) as TextBlock;
                if (String.IsNullOrEmpty(x.Text))
                    break;
                currentProjectPath = x.Text;
                AddMethodsForProjectInternal(currentProjectPath);
            }
        }

        private void AddMethodsForProjectInternal(string currentProjectPath)
        {
            //if (File.Exists(currentProjectPath))
            //{
            //    ATACore.Project.ProjectBuilder.BuildProject(currentProjectPath);
            //    string currentProjectDllPath = ATACore.Project.ProjectInfoCollector.GetAssemblyPathByProjectPath(currentProjectPath);
            //    MethodInfo[] currentDllMethods = ATACore.Project.ProjectInfoCollector.GetProjectTestMethods(currentProjectDllPath);
            //    AddMethodsToCheckBoxListInternal(currentProjectDllPath, currentDllMethods);
            //    testIds.AddRange(ATACore.TestExecution.TestListGenerator.GenerateTestMethodIds(currentDllMethods, currentProjectDllPath));
            //}
        }

        private List<string> FilterCheckedTests()
        {
            List<string> filteredTestIds = new List<string>();
            //for (int i = 0; i < dgvMethods.Rows.Count; i++)
            //{
            //    DataGridViewCheckBoxCell currentCheckBox = (DataGridViewCheckBoxCell)dgvMethods.Rows[i].Cells[0];
            //    if (Convert.ToBoolean(currentCheckBox.Value))
            //    {
            //        string currentChecked = dgvMethods.Rows[i].Cells[1].Value.ToString();
            //        int index = testIds.FindIndex(x => x.Equals(dgvMethods.Rows[i].Cells[3].Value.ToString()));
            //        if (index != -1)
            //        {
            //            filteredTestIds.Add(testIds[index]);
            //        }
            //    }
            //}
            return filteredTestIds;
        }

        private List<string> GetProjectPaths()
        {
            List<string> projectPaths = new List<string>();
            for (int i = 0; i < dgvProjects.Items.Count; i++)
            {
                string currentProjectPath = String.Empty;
                TextBlock x = dgvProjects.Columns[0].GetCellContent(dgvProjects.Items[i]) as TextBlock;
                if (String.IsNullOrEmpty(x.Text))
                    break;
                currentProjectPath = x.Text;
                if (File.Exists(currentProjectPath))
                    projectPaths.Add(currentProjectPath);
            }
            return projectPaths;
        }

        private void DisplayClientMessage(string agentMessage)
        {
            tbMainLog.Text += String.Concat(Environment.NewLine, Environment.NewLine, agentMessage);
        }

        private void DisplayMsBuildLogMessage(string agentMessage)
        {
            tbMsBuildLog.Text += String.Concat(Environment.NewLine, Environment.NewLine, agentMessage);
        }

        private void CheckAllTests()
        {
            //foreach (DataGridViewRow row in dgvMethods.Rows)
            //{
            //    row.Cells[0].Value = true;
            //}
        }

        private void backgroundWorker1_DoWork()
        {
            while (isConnected)
            {
                NetworkStream serverStream = clientTcpListner.GetStream();
                byte[] inStream = new byte[10025];
                serverStream.Read(inStream, 0, (int)clientTcpListner.ReceiveBufferSize);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                UpdateProgress uiUpdater = new UpdateProgress(DisplayClientMessage);
                if (returndata != String.Empty)
                {
                    this.Dispatcher.BeginInvoke(uiUpdater, returndata);
                }
                Thread.Sleep(1000);
            }
        }

        private void backgroundWorker2_DoWork()
        {
            lock (this)
            {
                IPAddress ipAddress = clientSettings.GetIPAddress();
                serverMsBuildUpdaterListener = new TcpListener(ipAddress, agentSettings.Port);
                // Gives a Signal to the initial thread that the agent client Listener is initialized
                Monitor.Pulse(this);
            }
            UpdateProgress uiUpdater1 = new UpdateProgress(DisplayClientMessage);
            serverMsBuildUpdaterListener.Start();
            clientMsBuildTcpAgentListner = default(TcpClient);
            clientMsBuildTcpAgentListner = serverMsBuildUpdaterListener.AcceptTcpClient();

            // Will update the status of the agents execution (MS Build Log) until the client is connected with the agent.
            UpdateProgress uiUpdater = new UpdateProgress(DisplayMsBuildLogMessage);
            while (isConnected)
            {
                NetworkStream serverStream = clientMsBuildTcpAgentListner.GetStream();
                byte[] inStream = new byte[10025];
                serverStream.Read(inStream, 0, (int)clientMsBuildTcpAgentListner.ReceiveBufferSize);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);

                if (returndata != String.Empty)
                {
                    this.Dispatcher.BeginInvoke(uiUpdater, returndata);
                    //this.Invoke(uiUpdater, returndata);
                }
            }
        }
        #endregion

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            isConnected = false;
            clientTcpWriter.Close();
            clientTcpListner.Close();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            //TestGetIpsInformation();
            GetIpsInformation();
            //backgroundWorker2.RunWorkerAsync();
            lock (this)
            {
                // Waits until the listener in the backgroundworker2 is initialized
                Monitor.Wait(this);
            }
            UpdateProgress uiUpdater = new UpdateProgress(DisplayClientMessage);
            {
                clientTcpWriter = new TcpClient();
                clientTcpListner = new TcpClient();
                lock (this)
                {
                    clientTcpWriter.Connect(agentSettings.IpString, clientSettings.Port);
                    Thread.Sleep(2000);
                    clientTcpListner.Connect(agentSettings.IpString, clientSettings.Port);
                }
                isConnected = true;
                tbMainLog.Text = "Client Socket Program - Server Connected ...";

                //backgroundWorker1.RunWorkerAsync();
            }
        }

        private void btnClean_Click(object sender, RoutedEventArgs e)
        {
            tbMainLog.Text = String.Empty;
            tbMsBuildLog.Text = String.Empty;
        }

        private void btnSendCustomCommand_Click(object sender, RoutedEventArgs e)
        {
            ATACore.TcpWrapperProcessor.TcpClientWrapper.SendMessageToClient(clientTcpWriter, tbCustomCommand.Text);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshInternal();
        }

        private void btnCreateWorkspace_Click(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < dgvWorkspaces.Rows.Count; i++)
            //{
            //    string currentTfsPath = dgvWorkspaces.Rows[i].Cells[0].Value.ToString();
            //    string currentLocalPath = dgvWorkspaces.Rows[i].Cells[1].Value.ToString();
            //    string currentCollection = dgvWorkspaces.Rows[i].Cells[2].Value.ToString();
            //    string currentServerUrl = String.Concat(tfsServerUrl.Text, currentCollection);
            //    string currentWorkspaceName = Guid.NewGuid().ToString();
            //    dgvWorkspaces.Rows[i].Cells[3].Value = currentWorkspaceName;
            //    string messageToBeSend = ATACore.CommandExecutor.GenerateCreateWorkspacetMessage(tfsProjFullPath, currentServerUrl, currentWorkspaceName, currentTfsPath, currentLocalPath, agentSettings);
            //    ATACore.TcpWrapperProcessor.TcpClientWrapper.SendMessageToClient(clientTcpWriter, messageToBeSend);
            //}      
        }

        private void btnDeleteWorkspace_Click(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < dgvWorkspaces.Rows.Count; i++)
            //{
            //    string currentTfsPath = dgvWorkspaces.Rows[i].Cells[1].Value.ToString();
            //    string currentLocalPath = dgvWorkspaces.Rows[i].Cells[2].Value.ToString();
            //    string currentCollection = dgvWorkspaces.Rows[i].Cells[2].Value.ToString();
            //    string currentServerUrl = String.Concat(tfsServerUrl.Text, currentCollection);
            //    string currentWorkspaceName = dgvWorkspaces.Rows[i].Cells[3].Value.ToString();
            //    string messageToBeSend = ATACore.CommandExecutor.GenerateDeleteWorkspacetMessage(tfsProjFullPath, currentServerUrl, currentWorkspaceName, tbUserName.Text, agentSettings);
            //    ATACore.TcpWrapperProcessor.TcpClientWrapper.SendMessageToClient(clientTcpWriter, messageToBeSend);
            //}
        }

        private void btnGetLatest_Click(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < dgvWorkspaces.Rows.Count; i++)
            //{
            //    string currentTfsPath = dgvWorkspaces.Rows[i].Cells[1].Value.ToString();
            //    string currentLocalPath = dgvWorkspaces.Rows[i].Cells[2].Value.ToString();
            //    string currentCollection = dgvWorkspaces.Rows[i].Cells[2].Value.ToString();
            //    string currentServerUrl = String.Concat(tfsServerUrl.Text, currentCollection);
            //    string messageToBeSend = ATACore.CommandExecutor.GenerateTfsGetLatestMessage(tfsProjFullPath, currentLocalPath, tbUserName.Text, agentSettings);
            //    ATACore.TcpWrapperProcessor.TcpClientWrapper.SendMessageToClient(clientTcpWriter, messageToBeSend);
            //}
        }

        private void dgvProjects_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            RefreshInternal();
            CheckAllTests();
        }
    }
}
