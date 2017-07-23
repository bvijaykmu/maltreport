﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Reflection;
using Fluid;
using Sandwych.Reporting.Textilize;
using System.Threading.Tasks;
using Sandwych.Reporting.OpenDocument.Filters;

namespace Sandwych.Reporting.OpenDocument
{
    public abstract class OdfTemplate : IDocumentTemplate
    {
        private readonly OdfDocument _document;
        private IFluidTemplate _fluidTemplate = null;

        public OdfTemplate(Stream inStream)
        {
            _document = new OdfDocument();
            _document.Load(inStream);
            this.CompileAndParse();
        }

        public OdfTemplate(string filePath)
        {
            _document = new OdfDocument();
            _document.Load(filePath);
            this.CompileAndParse();
        }

        public OdfTemplate(OdfDocument document)
        {
            _document = document;
            this.CompileAndParse();
        }

        public async Task<IDocument> RenderAsync(TemplateContext context)
        {
            var outputDocument = new OdfDocument();
            this._document.SaveAs(outputDocument);

            var mainContentTemplate = _document.ReadTextEntry(_document.MainContentEntryPath);

            this.SetInternalFilters(outputDocument, context.FluidContext);

            using (var ws = outputDocument.OpenOrCreateEntryToWrite(outputDocument.MainContentEntryPath))
            using (var writer = new StreamWriter(ws))
            {
                await _fluidTemplate.RenderAsync(writer, HtmlEncoder.Default, context.FluidContext);
            }

            outputDocument.Flush();
            return outputDocument;
        }

        protected virtual void SetInternalFilters(OdfDocument outputDocument, FluidTemplateContext templateContext)
        {
            var imageFilter = new OdfImageFilter(outputDocument);
            templateContext.Filters.AddFilter(imageFilter.Name, imageFilter.Execute);
        }

        public IDocument Render(TemplateContext context) =>
            this.RenderAsync(context).GetAwaiter().GetResult();

        private void CompileAndParse()
        {
            OdfCompiler.Compile(_document);
            _document.Flush();

            var mainContentText = _document.GetEntryTextReader(_document.MainContentEntryPath).ReadToEnd();
            if (!FluidTemplate.TryParse(mainContentText, out this._fluidTemplate, out var errors))
            {
                throw new SyntaxErrorException(errors.Aggregate((x, y) => x + "\n" + y));
            }
        }

    }
}
