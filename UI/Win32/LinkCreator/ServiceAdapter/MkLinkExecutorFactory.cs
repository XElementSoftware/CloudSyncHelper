﻿using System.ComponentModel.Composition;
using XElement.CloudSyncHelper.Logic.Execution.MkLink;

namespace XElement.CloudSyncHelper.UI.Win32.LinkCreator.ServiceAdapter
{
#region not unit-tested
    [Export( typeof( /*MkLink.*/IFactory ) )]
    internal class MkLinkExecutorFactory : /*MkLink.*/IFactory
    {
        private MkLinkExecutorFactory() { }


        public IExecutor Get( ParametersDTO parameters )
        {
            return new ExecutorProxy( parameters );
        }
    }
#endregion
}
