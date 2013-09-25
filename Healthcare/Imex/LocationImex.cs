#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("Location")]
	public class LocationImex : XmlEntityImex<Location, LocationImex.LocationData>
	{
		[DataContract]
		public class LocationData : ReferenceEntityDataBase
		{
			[DataMember]
			public string Id;

			[DataMember]
			public string Name;

			[DataMember]
			public string Description;

			[DataMember]
			public string FacilityCode;

			[DataMember]
			public string Building;

			[DataMember]
			public string Floor;

			[DataMember]
			public string PointOfCare;
		}

		#region Overrides

		protected override IList<Location> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			var where = new LocationSearchCriteria();
			where.Id.SortAsc(0);

			return context.GetBroker<ILocationBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override LocationData Export(Location entity, IReadContext context)
		{
			var data = new LocationData
						{
							Deactivated = entity.Deactivated,
							Id = entity.Id,
							Name = entity.Name,
							Description = entity.Description,
							FacilityCode = entity.Facility.Code,
							Building = entity.Building,
							Floor = entity.Floor,
							PointOfCare = entity.PointOfCare
						};

			return data;
		}

		protected override void Import(LocationData data, IUpdateContext context)
		{
			var facilityCriteria = new FacilitySearchCriteria();
			facilityCriteria.Code.EqualTo(data.FacilityCode);
			var facility = context.GetBroker<IFacilityBroker>().FindOne(facilityCriteria);

			var location = LoadOrCreateLocation(data.Id, data.Name, facility, context);
			location.Deactivated = data.Deactivated;
			location.Description = data.Description;
			location.Building = data.Building;
			location.Floor = data.Floor;
			location.PointOfCare = data.PointOfCare;
		}

		#endregion

		private Location LoadOrCreateLocation(string id, string name, Facility facility, IPersistenceContext context)
		{
			Location l;
			try
			{
				// see if already exists in db
				var where = new LocationSearchCriteria();
				where.Id.EqualTo(id);
				l = context.GetBroker<ILocationBroker>().FindOne(where);
			}
			catch (EntityNotFoundException)
			{
				// create it
				l = new Location(id, name, null, facility, null, null, null);
				context.Lock(l, DirtyState.New);
			}

			return l;
		}
	}
}
