﻿using System;
using System.ComponentModel.Composition;
using XElement.CloudSyncHelper.DataCreator.Model;
using XElement.CloudSyncHelper.Serialization.DataTypes;

namespace XElement.CloudSyncHelper.DataCreator.Data.Games
{
#region not unit-tested
    internal abstract class AbstractGameInfo : GameInfo, IPartImportsSatisfiedNotification
    {
        protected AbstractGameInfo( string guid )
        {
            this.Id = Guid.Parse( guid );
        }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            this.OnImportsSatisfied();
        }
        protected abstract void OnImportsSatisfied();

        [Import]
        protected DefinitionFactory _definitionFactory = null;

        [Import]
        protected OsConfigurationFactory _osConfigFactory = null;
    }
#endregion
}
