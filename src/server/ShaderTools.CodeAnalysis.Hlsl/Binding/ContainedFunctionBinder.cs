using System;
using System.Collections.Generic;

namespace ShaderTools.CodeAnalysis.Hlsl.Binding
{
    internal class ContainedFunctionBinder : Binder
    {
        private readonly Binder _containerBinder;

        public ContainedFunctionBinder(SharedBinderState sharedBinderState, Binder parent, Binder containerBinder)
            : base(sharedBinderState, parent)
        {
            if (containerBinder == null)
                throw new ArgumentNullException(nameof(containerBinder));
            _containerBinder = containerBinder;
        }

        protected override IEnumerable<Binder> GetAdditionalParentBinders()
        {
            return _containerBinder.GetBinderChain();
        }
    }
}