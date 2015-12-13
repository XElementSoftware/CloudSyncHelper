﻿using System.ComponentModel.Composition;

namespace XElement.CloudSyncHelper.DataCreator.Data.Games
{
    [Export( typeof( AbstractGameInfo ) )]
    internal class ClickerHeroes : AbstractGameInfo
    {
        [ImportingConstructor]
        public ClickerHeroes() : base( "359F7286-D6C6-426F-98ED-117B1EE49A0A" )
        {
            this.DisplayName = "Clicker Heroes";
            this.FolderName = "Clicker Heroes";
            this.TechnicalNameMatcher = "Clicker Heroes";
        }

        protected override void OnImportsSatisfied()
        {
            this.Configuration = this._configFactory.GetSteamCloud();
        }
    }
}
