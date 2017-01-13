﻿#region Copyright
//  Copyright 2016  OSIsoft, LLC
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
#endregion
using System;
using OSIsoft.AF;
using OSIsoft.AF.Asset;

namespace Ex4_Building_An_AF_Hierarchy_Sln
{
    class Program4
    {
        static void Main(string[] args)
        {
            AFDatabase database = GetDatabase("PISRV01", "Green Power Company");
            CreateElementTemplate(database);
            CreateFeedersRootElement(database);
            CreateFeederElements(database);
            CreateWeakReference(database);

            // This bonus exercise  creates a replica database
            //Bonus.Run();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static AFDatabase GetDatabase(string servername, string databasename)
        {
            PISystem system = GetPISystem(null, servername);
            if (!string.IsNullOrEmpty(databasename))
                return system.Databases[databasename];
            else
                return system.Databases.DefaultDatabase;
        }

        static PISystem GetPISystem(PISystems systems = null, string systemname = null)
        {
            systems = systems == null ? new PISystems() : systems;
            if (!string.IsNullOrEmpty(systemname))
                return systems[systemname];
            else
                return systems.DefaultPISystem;
        }

        static void CreateElementTemplate(AFDatabase database)
        {
            string templateName = "FeederTemplate";
            AFElementTemplate feederTemplate;
            if (database.ElementTemplates.Contains(templateName))
                return;
            else
                feederTemplate = database.ElementTemplates.Add(templateName);

            AFAttributeTemplate cityAttributeTemplate = feederTemplate.AttributeTemplates.Add("City");
            cityAttributeTemplate.Type = typeof(string);

            AFAttributeTemplate power = feederTemplate.AttributeTemplates.Add("Power");
            power.Type = typeof(Single);

            power.DefaultUOM = database.PISystem.UOMDatabase.UOMs["watt"];
            power.DataReferencePlugIn = database.PISystem.DataReferencePlugIns["PI Point"];

            database.CheckIn();
        }

        static void CreateFeedersRootElement(AFDatabase database)
        {
            if (database.Elements.Contains("Feeders"))
                return;
            else
            {
                database.Elements.Add("Feeders");
            }
            database.CheckIn();
        }

        static void CreateFeederElements(AFDatabase database)
        {
            AFElementTemplate template = database.ElementTemplates["FeederTemplate"];

            AFElement feeders = database.Elements["Feeders"];
            if (template == null || feeders == null) return;

            if (feeders.Elements.Contains("Feeder001")) return;
            AFElement feeder001 = feeders.Elements.Add("Feeder001", template);

            AFAttribute city = feeder001.Attributes["City"];
            if (city != null) city.SetValue(new AFValue("London"));

            AFAttribute power = feeder001.Attributes["Power"];
            power.ConfigString = @"\\PISRV01\SINUSOID";

            database.CheckIn();
        }

        static void CreateWeakReference(AFDatabase database)
        {
            AFReferenceType weakRefType = database.ReferenceTypes["Weak Reference"];

            AFElement hogwarts = database.Elements["Geographical Locations"].Elements["London"];
            AFElement feeder0001 = database.Elements["Feeders"].Elements["Feeder001"];
            if (hogwarts == null || feeder0001 == null) return;

            hogwarts.Elements.Add(feeder0001, weakRefType);

            database.CheckIn();
        }
    }
}
