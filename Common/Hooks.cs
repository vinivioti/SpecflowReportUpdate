using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using FrameVioti.Support;
using System;
using System.IO;
using TechTalk.SpecFlow;
using DinkToPdf;
using DinkToPdf.Contracts;
using FrameVioti.ExtentionMethods;
using OpenQA.Selenium;
using FrameVioti.GerenciadorDriver;
using TechTalk.SpecFlow.Bindings;
using System.Diagnostics;
using FluentAssertions.Specialized;

namespace FrameVioti.Common
{
    [Binding]
    public class Hooks
    {
        private static ExtentTest? _feature;
        private static ExtentTest? _scenario;
        private static ExtentReports? _extent;
        private static readonly string basePath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string reportersPath = Path.Combine(basePath, "Reporters");
        private static readonly string reportDateTime = DateTime.Now.ToString("ddMMyyyy_HHmmss");
        private static readonly string reportFileName = $"ReportFrame_{reportDateTime}.html";
        private static readonly string reportPath = Path.Combine(reportersPath, reportFileName);
        private static readonly IConverter? _converter = new SynchronizedConverter(new PdfTools());

     

        [BeforeTestRun]
        public static void ConfigureReport()
        {
            if (!Directory.Exists(reportersPath))
                Directory.CreateDirectory(reportersPath);

            var reporter = new ExtentSparkReporter(reportPath);

            // Configurações do relatório
            reporter.Config.Theme = AventStack.ExtentReports.Reporter.Config.Theme.Dark;// ou Standard = Light
           // reporter.Config.Theme = AventStack.ExtentReports.Reporter.Config.Theme.Standard;// ou Dark
            reporter.Config.ReportName = "Relatório Teste Vioti";
            reporter.Config.DocumentTitle = "Realtório Geral - Projeto Frame Vioti";
            reporter.Config.TimelineEnabled = false;
 


            Console.WriteLine($"Caminho do arquivo de relatório: {reportPath}");

            _extent = new ExtentReports();
            _extent.AttachReporter(reporter);
            Console.WriteLine("ExtentReports configurado com sucesso.");
                
        }

        [BeforeFeature]
        [Obsolete]
        public static void CreateFeature()
        {
            if (_extent != null)
            {
                _feature = _extent.CreateTest<Feature>(FeatureContext.Current.FeatureInfo.Title);
            }
        }

        [BeforeScenario]
        [Obsolete]
        public static void CreateScenario()
        {
            if (_feature != null)
            {
                _scenario = _feature.CreateNode<Scenario>(ScenarioContext.Current.ScenarioInfo.Title);

            }
        }

        [AfterStep]
        [Obsolete]
        public static void InsertReportingSteps()
        {
            if (_scenario != null)
            {
                switch (ScenarioStepContext.Current.StepInfo.StepDefinitionType)
                {
                    case StepDefinitionType.Given:
                        _scenario.StepDefinitionGiven();
                        break;
                    case StepDefinitionType.When:
                        _scenario.StepDefinitionWhen();
                        break;
                    case StepDefinitionType.Then:
                        _scenario.StepDefinitionThen();
                        break;
                }
                // Adiciona tratamento de falhas
                if (ScenarioContext.Current.TestError != null)
                {
                    _scenario.Fail(ScenarioContext.Current.TestError.Message);
                }

                Console.WriteLine($"Passo reportado: {_scenario.Model.Name}");
            }
        }
        

        [AfterTestRun]
        public static void FlushExtent()
        {

            if (_extent != null)
            {
                _extent.Flush();
            }
            
            var indexFilePath = Path.Combine(reportersPath, "index.html");
            var desiredFilePath = Path.Combine(reportersPath, reportFileName);

            if (File.Exists(indexFilePath))
            {
                File.Move(indexFilePath, desiredFilePath);
            }

            Console.WriteLine($"Relatório gerado: {desiredFilePath}");

            if (File.Exists(desiredFilePath))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = desiredFilePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao abrir o relatório HTML: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("O arquivo HTML não foi encontrado para abrir.");
            }

            DriverFactory.KillDriver();
        }
    }
}
