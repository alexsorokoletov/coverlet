﻿// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using coverlet.collector.Resources;
using Coverlet.Collector.Utilities;

namespace Coverlet.Collector.DataCollection
{
    /// <summary>
    /// Coverlet settings parser
    /// </summary>
    internal class CoverletSettingsParser
    {
        private readonly TestPlatformEqtTrace _eqtTrace;

        public CoverletSettingsParser(TestPlatformEqtTrace eqtTrace)
        {
            _eqtTrace = eqtTrace;
        }

        /// <summary>
        /// Parser coverlet settings
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <param name="testModules">Test modules</param>
        /// <returns>Coverlet settings</returns>
        public CoverletSettings Parse(XmlElement configurationElement, IEnumerable<string> testModules)
        {
            var coverletSettings = new CoverletSettings
            {
                TestModule = ParseTestModule(testModules)
            };

            if (configurationElement != null)
            {
                coverletSettings.IncludeFilters = ParseIncludeFilters(configurationElement);
                coverletSettings.IncludeDirectories = ParseIncludeDirectories(configurationElement);
                coverletSettings.ExcludeAttributes = ParseExcludeAttributes(configurationElement);
                coverletSettings.ExcludeSourceFiles = ParseExcludeSourceFiles(configurationElement);
                coverletSettings.MergeWith = ParseMergeWith(configurationElement);
                coverletSettings.UseSourceLink = ParseUseSourceLink(configurationElement);
                coverletSettings.SingleHit = ParseSingleHit(configurationElement);
                coverletSettings.IncludeTestAssembly = ParseIncludeTestAssembly(configurationElement);
                coverletSettings.SkipAutoProps = ParseSkipAutoProps(configurationElement);
                coverletSettings.DoesNotReturnAttributes = ParseDoesNotReturnAttributes(configurationElement);
                coverletSettings.DeterministicReport = ParseDeterministicReport(configurationElement);
                coverletSettings.InstrumentModulesWithoutLocalSources = ParseInstrumentModulesWithoutLocalSources(configurationElement);
            }

            coverletSettings.ReportFormats = ParseReportFormats(configurationElement);
            coverletSettings.ExcludeFilters = ParseExcludeFilters(configurationElement);

            if (_eqtTrace.IsVerboseEnabled)
            {
                _eqtTrace.Verbose("{0}: Initializing coverlet process with settings: \"{1}\"", CoverletConstants.DataCollectorName, coverletSettings.ToString());
            }

            return coverletSettings;
        }

        /// <summary>
        /// Parses test module
        /// </summary>
        /// <param name="testModules">Test modules</param>
        /// <returns>Test module</returns>
        private static string ParseTestModule(IEnumerable<string> testModules)
        {
            // Validate if at least one source present.
            if (testModules == null || !testModules.Any())
            {
                string errorMessage = string.Format(Resources.NoTestModulesFound, CoverletConstants.DataCollectorName);
                throw new CoverletDataCollectorException(errorMessage);
            }

            // Note:
            // 1) .NET core test run supports one testModule per run. Coverlet also supports one testModule per run. So, we are using first testSource only and ignoring others.
            // 2) If and when .NET full is supported with coverlet OR .NET core starts supporting multiple testModules, revisit this code to use other testModules as well.
            return testModules.FirstOrDefault();
        }

        /// <summary>
        /// Parse report formats
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Report formats</returns>
        private static string[] ParseReportFormats(XmlElement configurationElement)
        {
            string[] formats = Array.Empty<string>();
            if (configurationElement != null)
            {
                XmlElement reportFormatElement = configurationElement[CoverletConstants.ReportFormatElementName];
                formats = SplitElement(reportFormatElement);
            }

            return formats is null || formats.Length == 0 ? new[] { CoverletConstants.DefaultReportFormat } : formats;
        }

        /// <summary>
        /// Parse filters to include
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Filters to include</returns>
        private static string[] ParseIncludeFilters(XmlElement configurationElement)
        {
            XmlElement includeFiltersElement = configurationElement[CoverletConstants.IncludeFiltersElementName];
            return SplitElement(includeFiltersElement);
        }

        /// <summary>
        /// Parse directories to include
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Directories to include</returns>
        private static string[] ParseIncludeDirectories(XmlElement configurationElement)
        {
            XmlElement includeDirectoriesElement = configurationElement[CoverletConstants.IncludeDirectoriesElementName];
            return SplitElement(includeDirectoriesElement);
        }

        /// <summary>
        /// Parse filters to exclude
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Filters to exclude</returns>
        private static string[] ParseExcludeFilters(XmlElement configurationElement)
        {
            var excludeFilters = new List<string> { CoverletConstants.DefaultExcludeFilter };

            if (configurationElement != null)
            {
                XmlElement excludeFiltersElement = configurationElement[CoverletConstants.ExcludeFiltersElementName];
                string[] filters = SplitElement(excludeFiltersElement);
                if (filters != null)
                {
                    excludeFilters.AddRange(filters);
                }
            }

            return excludeFilters.ToArray();
        }

        /// <summary>
        /// Parse source files to exclude
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Source files to exclude</returns>
        private static string[] ParseExcludeSourceFiles(XmlElement configurationElement)
        {
            XmlElement excludeSourceFilesElement = configurationElement[CoverletConstants.ExcludeSourceFilesElementName];
            return SplitElement(excludeSourceFilesElement);
        }

        /// <summary>
        /// Parse attributes to exclude
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Attributes to exclude</returns>
        private static string[] ParseExcludeAttributes(XmlElement configurationElement)
        {
            XmlElement excludeAttributesElement = configurationElement[CoverletConstants.ExcludeAttributesElementName];
            return SplitElement(excludeAttributesElement);
        }

        /// <summary>
        /// Parse merge with attribute
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Merge with attribute</returns>
        private static string ParseMergeWith(XmlElement configurationElement)
        {
            XmlElement mergeWithElement = configurationElement[CoverletConstants.MergeWithElementName];
            return mergeWithElement?.InnerText;
        }

        /// <summary>
        /// Parse use source link flag
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Use source link flag</returns>
        private static bool ParseUseSourceLink(XmlElement configurationElement)
        {
            XmlElement useSourceLinkElement = configurationElement[CoverletConstants.UseSourceLinkElementName];
            bool.TryParse(useSourceLinkElement?.InnerText, out bool useSourceLink);
            return useSourceLink;
        }

        /// <summary>
        /// Parse single hit flag
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Single hit flag</returns>
        private static bool ParseSingleHit(XmlElement configurationElement)
        {
            XmlElement singleHitElement = configurationElement[CoverletConstants.SingleHitElementName];
            bool.TryParse(singleHitElement?.InnerText, out bool singleHit);
            return singleHit;
        }

        /// <summary>
        /// Parse ParseDeterministicReport flag
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>ParseDeterministicReport flag</returns>
        private static bool ParseDeterministicReport(XmlElement configurationElement)
        {
            XmlElement deterministicReportElement = configurationElement[CoverletConstants.DeterministicReport];
            bool.TryParse(deterministicReportElement?.InnerText, out bool deterministicReport);
            return deterministicReport;
        }

        /// <summary>
        /// Parse InstrumentModulesWithoutLocalSources flag
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>InstrumentModulesWithoutLocalSources flag</returns>
        private static bool ParseInstrumentModulesWithoutLocalSources(XmlElement configurationElement)
        {
            XmlElement instrumentModulesWithoutLocalSourcesElement = configurationElement[CoverletConstants.InstrumentModulesWithoutLocalSources];
            bool.TryParse(instrumentModulesWithoutLocalSourcesElement?.InnerText, out bool instrumentModulesWithoutLocalSources);
            return instrumentModulesWithoutLocalSources;
        }

        /// <summary>
        /// Parse include test assembly flag
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Include Test Assembly Flag</returns>
        private static bool ParseIncludeTestAssembly(XmlElement configurationElement)
        {
            XmlElement includeTestAssemblyElement = configurationElement[CoverletConstants.IncludeTestAssemblyElementName];
            bool.TryParse(includeTestAssemblyElement?.InnerText, out bool includeTestAssembly);
            return includeTestAssembly;
        }

        /// <summary>
        /// Parse skipautoprops flag
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Include Test Assembly Flag</returns>
        private static bool ParseSkipAutoProps(XmlElement configurationElement)
        {
            XmlElement skipAutoPropsElement = configurationElement[CoverletConstants.SkipAutoProps];
            bool.TryParse(skipAutoPropsElement?.InnerText, out bool skipAutoProps);
            return skipAutoProps;
        }

        /// <summary>
        /// Parse attributes that mark methods that do not return.
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>DoesNotReturn attributes</returns>
        private static string[] ParseDoesNotReturnAttributes(XmlElement configurationElement)
        {
            XmlElement doesNotReturnAttributesElement = configurationElement[CoverletConstants.DoesNotReturnAttributesElementName];
            return SplitElement(doesNotReturnAttributesElement);
        }

        /// <summary>
        /// Splits a comma separated elements into an array
        /// </summary>
        /// <param name="element">The element to split</param>
        /// <returns>An array of the values in the element</returns>
        private static string[] SplitElement(XmlElement element)
        {
            return element?.InnerText?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(value => !string.IsNullOrWhiteSpace(value)).Select(value => value.Trim()).ToArray();
        }
    }
}
