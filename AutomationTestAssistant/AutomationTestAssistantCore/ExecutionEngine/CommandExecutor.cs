﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using AutomationTestAssistantCore.ExecutionEngine.Messages;
using System.Xml.Serialization;
using System.Xml;
using AutomationTestAssistantCore.ExecutionEngine.Contracts;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AutomationTestAssistantCore
{
    public class CommandExecutor
    {
        private Process currentlyExecutedProcess;
        private Queue<string> commandsQueue;

        public CommandExecutor()
        {
            commandsQueue = new Queue<string>();
            currentlyExecutedProcess = new Process();
        }

        public Process QueueCommandToExecute(string xml)
        {
            Command command = GetCommandToBeExecutedFromMessage(xml);
            XmlSerializer deserializer;
            StringReader textReader = new StringReader(xml);
              using (textReader)
              {
                switch (command)
                {
                    //MST == Execute Ms Test
                    case Command.MST:
                        deserializer = new XmlSerializer(typeof(MessageArgsMsTest));
                        MessageArgsMsTest msTestArgs = (MessageArgsMsTest)deserializer.Deserialize(textReader);
                        currentlyExecutedProcess = ATACore.CommandLine.CommandLineExecutor.ExecuteMsTest(msTestArgs);
                        break;
                    case Command.MSTL:
                        deserializer = new XmlSerializer(typeof(MessageArgsMsTest));
                        MessageArgsMsTest msTestListArgs = (MessageArgsMsTest)deserializer.Deserialize(textReader);
                        currentlyExecutedProcess = ATACore.CommandLine.CommandLineExecutor.ExecuteMsTestSpecificList(msTestListArgs);
                        break;
                    //TFSWN == Execute TFS Create new Workspace
                    case Command.TFSWN:
                        deserializer = new XmlSerializer(typeof(MessageArgsWorkspaceCreation));
                        MessageArgsWorkspaceCreation tfsWorkspaceCreationArgs = (MessageArgsWorkspaceCreation)deserializer.Deserialize(textReader);
                        currentlyExecutedProcess = ATACore.CommandLine.CommandLineExecutor.ExecuteTfsCreateNewWorkspace(tfsWorkspaceCreationArgs);
                        break;
                    //TFSDW == Deletes TFS Workspace
                    case Command.TFSWD:
                        deserializer = new XmlSerializer(typeof(MessageArgsWorkspaceDeletion));
                        MessageArgsWorkspaceDeletion tfsWorkspaceDeletionArgs = (MessageArgsWorkspaceDeletion)deserializer.Deserialize(textReader);
                        currentlyExecutedProcess = ATACore.CommandLine.CommandLineExecutor.ExecuteTfsDeleteWorkspace(tfsWorkspaceDeletionArgs);
                        break;
                    //TFGL == TFS Get Latest
                    case Command.TFGL:
                        deserializer = new XmlSerializer(typeof(MessageArgsTfsGettingLatest));
                        MessageArgsTfsGettingLatest tfsGettingLatestArgs = (MessageArgsTfsGettingLatest)deserializer.Deserialize(textReader);
                        currentlyExecutedProcess = ATACore.CommandLine.CommandLineExecutor.ExecuteTfsGetLatest(tfsGettingLatestArgs);
                        break;
                    case Command.BUILD:
                        deserializer = new XmlSerializer(typeof(MessageArgsBuild));
                        MessageArgsBuild buildArgs = (MessageArgsBuild)deserializer.Deserialize(textReader);
                        currentlyExecutedProcess = ATACore.CommandLine.CommandLineExecutor.ExecuteBuild(buildArgs);
                        break;
                    case Command.DEL:
                        deserializer = new XmlSerializer(typeof(MessageArgsDelete));
                        MessageArgsDelete delArgs = (MessageArgsDelete)deserializer.Deserialize(textReader);
                        //ATACore.Utilities.FilesDeleter.DeleteAllFilesAndFolders(delArgs.LocalPath);
                        break;

                    default:
                        break;
                }
              }

            return currentlyExecutedProcess;
        }

        public Command GetCommandToBeExecutedFromMessage(string message)
        {
            string commandValue = GetCurrentCommandInternal(message);
            Command command = (Command)Enum.Parse(typeof(Command), commandValue);
            return command;
        }

        private string GetCurrentCommandInternal(string message)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(message);
            XmlNode commandElement = xmlDoc.GetElementsByTagName("Command")[0];

            return commandElement.InnerText;
        }

        public string GenerateCurrentCommandParametersXml(Object obj)
        {
            StringWriter writerStream = new StringWriter();
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
            string xml = String.Empty;
            using (writerStream)
            {
                xmlSerializer.Serialize(writerStream, obj);
                xml = writerStream.ToString();
            }
            xml = String.Concat(xml, "$$");
            return xml;
        }

        public Object MessageDeserializer(string str, Type type)
        {
            StringReader sr = new StringReader(str);
            Object returnObject;
            using(sr)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(type);
                returnObject = xmlSerializer.Deserialize(sr);
            }
            return returnObject;
        }
      
        public string GenerateMsTestMessage(string projectPath, string testListContent, string resultsFilePath, string testListName, IpAddressSettings ipAddressSettings, string workingDir)
        {
            MessageArgsMsTest messageArgs = new MessageArgsMsTest(Command.MSTL, projectPath, ipAddressSettings, workingDir, testListContent, testListName, resultsFilePath);

            string result = GenerateCurrentCommandParametersXml(messageArgs);
            return result;
        }

        public string GenerateResultParseMessage(string resultsFilePath, string currentUserName, IpAddressSettings ipAddressSettings)
        {
            MessageArgsParseResult messageArgs = new MessageArgsParseResult(resultsFilePath, currentUserName, ipAddressSettings);

            string result = GenerateCurrentCommandParametersXml(messageArgs);
            return result;
        }

        public string GenerateCreateWorkspacetMessage(string projectPath, string tfsServerUrl, string workspaceName, string tfsPath, string localPath, IpAddressSettings ipAddressSettings)
        {
            MessageArgsWorkspaceCreation messageArgs = new MessageArgsWorkspaceCreation(Command.TFSWN, projectPath, tfsServerUrl, ipAddressSettings, workspaceName, tfsPath, localPath);

            string result = GenerateCurrentCommandParametersXml(messageArgs);
            return result;
        }

        public string GenerateDeleteWorkspacetMessage(string projectPath, string tfsServerUrl, string workspaceName, string userName, IpAddressSettings ipAddressSettings)
        {
            MessageArgsWorkspaceDeletion messageArgs = new MessageArgsWorkspaceDeletion(Command.TFSWD, projectPath, ipAddressSettings, tfsServerUrl, workspaceName, userName);
            string result = GenerateCurrentCommandParametersXml(messageArgs);
            return result;
        }

        public string GenerateTfsGetLatestMessage(string projectPath, string pathToGet, string userName, IpAddressSettings ipAddressSettings)
        {
            MessageArgsTfsGettingLatest messageArgs = new MessageArgsTfsGettingLatest(Command.TFGL, projectPath, ipAddressSettings, pathToGet, userName);
            string result = GenerateCurrentCommandParametersXml(messageArgs);
            return result;
        }

        public string GenerateDeleteMessage(string localPath, IpAddressSettings ipAddressSettings)
        {
            MessageArgsDelete messageArgs = new MessageArgsDelete(localPath, ipAddressSettings);
            string result = GenerateCurrentCommandParametersXml(messageArgs);
            return result;
        }

        public string GenerateBuildMessage(string localPath, string name, IpAddressSettings ipAddressSettings)
        {
            MessageArgsBuild messageArgs = new MessageArgsBuild(localPath, name, ipAddressSettings);
            string result = GenerateCurrentCommandParametersXml(messageArgs);
            return result;
        }

        public void EnqueueNewMessage(string dequeuedMsg, MessageSource messageSource, ConcurrentQueue<MessageArgsLogger> messagesToBeSend)
        {
            messagesToBeSend.Enqueue(new MessageArgsLogger(messageSource, dequeuedMsg));
        }
    }
}
