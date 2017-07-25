﻿using System;
using System.Collections.Generic;
using Sandwych.Reporting.OpenDocument;
using Sandwych.Reporting.Tests.Common;
using Xunit;

namespace Sandwych.Reporting.Tests.OpenDocument
{

    public class OdsTemplateTest
    {
        private const string Template2OdsName = "Sandwych.Reporting.Tests.OpenDocument.Templates.Template2.ods";

        [Fact]
        public void CanCompileOdsDocumentTemplate()
        {
            using (var stream = DocumentTestHelper.GetResource(Template2OdsName))
            {
                var ods = OdfDocument.Load(stream);
                var template = new OdsTemplate(ods);
            }
        }

        [Fact]
        public void CanRenderOdsTemplate()
        {
            OdfTemplate template;
            using (var stream = DocumentTestHelper.GetResource(Template2OdsName))
            {
                var ods = OdfDocument.Load(stream);
                template = new OdsTemplate(ods);
            }

            var dataSet = new TestingDataSet();
            var values = new Dictionary<string, object>()
            {
                { "table1", dataSet.Table1 },
                { "so", dataSet.SimpleObject },
            };
            var context = new TemplateContext(values);

            var result = template.Render(context);

            result.Save(@"c:\tmp\out.ods");
        }

    }
}