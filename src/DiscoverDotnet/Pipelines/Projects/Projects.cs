﻿using System;
using System.Collections.Generic;
using System.Text;
using DiscoverDotnet.Modules;
using Microsoft.Extensions.Configuration;
using Statiq.Common;
using Statiq.Core;
using Statiq.Html;
using Statiq.Json;
using Statiq.Razor;
using Statiq.Yaml;

namespace DiscoverDotnet.Pipelines.Projects
{
    public class Projects : Pipeline
    {
        public Projects()
        {
            InputModules = new ModuleList
            {
                new ReadFiles(Config.FromContext(ctx => ctx.FileSystem.RootPath.Combine("../../data/projects/*.yml").FullPath)),
                new ExecuteIf(Config.FromContext(ctx => ctx.ApplicationState.IsCommand("preview") || ctx.Settings.GetBool("dev")))
                {
                    new OrderDocuments(Config.FromDocument(x => x.Source)),
                    new TakeDocuments(10)
                }
            };

            ProcessModules = new ModuleList
            {
                new LogMessage(Config.FromContext(ctx => $"Getting project data for {ctx.Inputs.Length} projects...")),
                new ParseYaml(),
                new SetContent(string.Empty),
                new GetProjectData(),
                new SetDestination(Config.FromDocument(x => (FilePath)$"projects/{x.Source.FileName.ChangeExtension("html")}")),
                new SetMetadata("Key", Config.FromDocument(x => x.Source.FileNameWithoutExtension.FullPath)),
                new SetMetadata("Link", Config.FromDocument((d, c) => c.GetLink(d))),
                new SetMetadata("DonationsData", Config.FromDocument(x => x.GetMetadata(
                    "Website",
                    "NuGet",
                    "SourceCode",
                    "Destination"))),
                new SetMetadata("CardData", Config.FromDocument(x => x.GetMetadata(
                    "Key",
                    "Title",
                    "Link",
                    "Image",
                    "NuGet",
                    "SourceCode",
                    "Description",
                    "StargazersCount",
                    "ForksCount",
                    "OpenIssuesCount",
                    "PushedAt",
                    "Website",
                    "Donations",
                    "Language",
                    "Tags",
                    "DiscoveryDate",
                    "Comment",
                    "Platform",
                    "Microsoft",
                    "Foundation")))
            };

            TransformModules = new ModuleList
            {
                new RenderRazor().WithLayout((FilePath)"/projects/_layout.cshtml"),
                new MirrorResources()
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
