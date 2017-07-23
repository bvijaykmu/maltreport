﻿using System;
using System.Collections.Generic;
using System.Text;
using Fluid;

namespace Sandwych.Reporting.Textilize
{
    public class FluidTemplateContext : Fluid.TemplateContext
    {
        public FluidTemplateContext(IReadOnlyDictionary<string, object> context)
        {
            this.MemberAccessStrategy = new UnsafeMemberAccessStrategy(GlobalMemberAccessStrategy);
            foreach (var pair in context)
            {
                this.SetValue(pair.Key, Fluid.Values.FluidValue.Create(pair.Value));
                if (pair.Value != null)
                {
                    this.MemberAccessStrategy.Register(pair.Value.GetType());
                }
            }
        }
    }
}
