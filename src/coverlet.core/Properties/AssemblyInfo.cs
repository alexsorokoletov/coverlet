// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using System.Runtime.CompilerServices;

// [assembly: AssemblyKeyFile("coverlet.core.snk")]

[assembly: InternalsVisibleTo("coverlet.msbuild.tasks")]
[assembly: InternalsVisibleTo("coverlet.console")]
[assembly: InternalsVisibleTo("coverlet.collector")]

[assembly: InternalsVisibleTo("coverlet.core.tests")]
[assembly: InternalsVisibleTo("coverlet.collector.tests")]
[assembly: InternalsVisibleTo("coverlet.integration.tests")]
// Needed to mock internal type https://github.com/Moq/moq4/wiki/Quickstart#advanced-features
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("CodeCoverage.Core.Coverlet")]