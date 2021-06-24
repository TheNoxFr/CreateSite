using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Axl;

namespace CreateSite
{
    public class Cisco
    {
        public string Url { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        private AXLAPIService client;

        public Cisco(string url, string login, string password)
        {
            Url = url;
            Login = login;
            Password = password;
        }

        public void Init()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            client = new AXLAPIService();

            client.Url = Url;
            client.Credentials = new NetworkCredential(Login, Password);
        }

        public bool AddPartition(string name, string description)
        {
            bool result = true;

            AddRoutePartitionReq req = new AddRoutePartitionReq();
            req.routePartition = new XRoutePartition();
            req.routePartition.name = name;
            req.routePartition.description = description;

            StandardResponse res = client.addRoutePartition(req);

            return result;
        }
        
        public bool AddCCMGroup(string name, List<string> members)
        {
            bool result = true;

            AddCallManagerGroupReq req = new AddCallManagerGroupReq();
            req.callManagerGroup = new XCallManagerGroup();
            req.callManagerGroup.name = name;
            req.callManagerGroup.members = new XCallManagerGroupMembers();
            req.callManagerGroup.members.member = new XCallManagerMember[members.Count];

            int idx = 0;
            foreach(string member in members)
            {
                req.callManagerGroup.members.member[idx] = new XCallManagerMember();
                req.callManagerGroup.members.member[idx].callManagerName = new XFkType();
                req.callManagerGroup.members.member[idx].callManagerName.Value = member;
                req.callManagerGroup.members.member[idx].priority = idx.ToString();
                idx++;
            }

            StandardResponse res = client.addCallManagerGroup(req);

            return result;
        }

        public bool AddDateTimeGroup(string name)
        {
            bool result = true;

            AddDateTimeGroupReq req = new AddDateTimeGroupReq();
            req.dateTimeGroup = new XDateTimeGroup();
            req.dateTimeGroup.name = name;
            req.dateTimeGroup.timeZone = "Europe/Paris";
            req.dateTimeGroup.separator = "/";
            req.dateTimeGroup.dateformat = "D/M/Y";
            req.dateTimeGroup.timeFormat = "24-hour";

            StandardResponse res = client.addDateTimeGroup(req);

            return result;
        }

        public bool AddMRGL(string name)
        {
            bool result = true;

            AddMediaResourceListReq req = new AddMediaResourceListReq();
            req.mediaResourceList = new XMediaResourceList();
            req.mediaResourceList.name = name;

            StandardResponse res = client.addMediaResourceList(req);

            return result;
        }

        public bool AddLocalRouteGroupName(string name, string description)
        {
            bool result = true;

            AddLocalRouteGroupReq req = new AddLocalRouteGroupReq();
            req.localRouteGroup = new XLocalRouteGroup();
            req.localRouteGroup.name = name;
            req.localRouteGroup.description = description;

            StandardResponse res = client.addLocalRouteGroup(req);

            return result;
        }

        public bool UpdateLocalRouteGroupName(string oldname, string newname, string description)
        {
            bool result = true;

            UpdateLocalRouteGroupReq req = new UpdateLocalRouteGroupReq();
            req.localRouteGroup = new UpdateLocalRouteGroupReqLocalRouteGroup();
            req.localRouteGroup.name = oldname;
            req.localRouteGroup.newName = newname;
            req.localRouteGroup.newDescription = description;

            StandardResponse res = client.updateLocalRouteGroup(req);

            return result;
        }

        public bool AddApplicationUser(string name,string password, List<string> accessgroups)
        {
            bool result = true;

            AddAppUserReq req = new AddAppUserReq();
            req.appUser = new XAppUser();
            req.appUser.userid = name;
            req.appUser.password = password;
            req.appUser.associatedGroups = new XAppUserUserGroup[accessgroups.Count];

            int idx = 0;
            foreach (string ag in accessgroups)
            {
                req.appUser.associatedGroups[idx] = new XAppUserUserGroup();
                req.appUser.associatedGroups[idx].name = ag;
                idx++;
            }

            StandardResponse res = client.addAppUser(req);
            return result;
        }
        
        public bool AddDeviceMobiltyGroup(string name)
        {
            bool result = true;

            AddDeviceMobilityGroupReq req = new AddDeviceMobilityGroupReq();
            req.deviceMobilityGroup = new XDeviceMobilityGroup();
            req.deviceMobilityGroup.name = name;

            StandardResponse res = client.addDeviceMobilityGroup(req);

            return result;
        }
        
        // Existe de base : pas utile
        public bool AddSIPTrunkSecurityProfile(string name)
        {
            bool result = true;


            return result;
        }

        // Existe de base : pas utile
        public bool AddSIPProfile(string name)
        {
            bool result = true;


            return result;
        }

        public bool AddCSS(string name, List<string> partitions)
        {
            bool result = true;

            AddCssReq req = new AddCssReq();
            req.css = new XCss();
            req.css.name = name;
            req.css.members = new XCssMembers();
            req.css.members.member = new XCallingSearchSpaceMember[partitions.Count];
            int idx = 0;
            foreach (string partition in partitions)
            {
                req.css.members.member[idx] = new XCallingSearchSpaceMember();
                req.css.members.member[idx].routePartitionName = new XFkType { Value = partition };
                req.css.members.member[idx].index = idx.ToString();
                idx++;
            }

            StandardResponse res = client.addCss(req);
            return result;
        }

        public GetCssRes GetCSS(string name)
        {
            GetCssReq req = new GetCssReq();
            req.ItemElementName = ItemChoiceType59.name;
            req.Item = name;

            GetCssRes res = null;
            try
            {
                res = client.getCss(req);
            }
            catch (Exception)
            {
            }

            return res;
        }

        public bool AddDevicePool(string name, string ccmg, string datetimegroup, string region, string mrgl, string location, string physlocation, string devicemobilitygroup, string primarylrg, string secondarylrg,string devicemobilitycss, string geolocation)
        {
            bool result = true;

            //GetDevicePoolReq r = new GetDevicePoolReq { ItemElementName = ItemChoiceType70.name, Item = "S01_S1a-S1b-S1c_DP" };
            //GetDevicePoolRes re = client.getDevicePool(r);

            AddDevicePoolReq req = new AddDevicePoolReq();
            req.devicePool = new XDevicePool();
            req.devicePool.name = name;
            req.devicePool.networkLocale = "France";
            req.devicePool.callManagerGroupName = new XFkType { Value = ccmg };
            req.devicePool.dateTimeSettingName = new XFkType { Value = datetimegroup };
            req.devicePool.regionName = new XFkType { Value = region };
            req.devicePool.mediaResourceListName = new XFkType { Value = mrgl };
            req.devicePool.locationName = new XFkType { Value = location };
            req.devicePool.physicalLocationName = new XFkType { Value = physlocation };
            req.devicePool.deviceMobilityGroupName = new XFkType { Value = devicemobilitygroup };
            req.devicePool.mobilityCssName = new XFkType { Value = devicemobilitycss };
            req.devicePool.geoLocationName = new XFkType { Value = geolocation };
            req.devicePool.localRouteGroup = new XDevicePoolLocalRouteGroup[2];
            req.devicePool.localRouteGroup[0] = new XDevicePoolLocalRouteGroup { name = "Primary Local Route Group", value = primarylrg };
            req.devicePool.localRouteGroup[1] = new XDevicePoolLocalRouteGroup { name = "Secondary Local Route Group", value = secondarylrg };


            StandardResponse res = client.addDevicePool(req);
            return result;
        }


        public bool AddTrunk(string name, string destination)
        {
            bool result = true;

            AddSipTrunkReq req = new AddSipTrunkReq();
            req.sipTrunk = new XSipTrunk();
            req.sipTrunk.name = name;
            req.sipTrunk.product = "SIP Trunk";
            req.sipTrunk.@class = "Trunk";
            req.sipTrunk.protocol = "SIP";
            req.sipTrunk.protocolSide = "Network"; // ??
            req.sipTrunk.service = "";
            req.sipTrunk.description = name;
            req.sipTrunk.devicePoolName = new XFkType();
            req.sipTrunk.devicePoolName.Value = "SIP_TK_DP";
            req.sipTrunk.locationName = new XFkType();
            req.sipTrunk.locationName.Value = "Hub_None";
            req.sipTrunk.destinations = new XSipTrunkDestinations();
            req.sipTrunk.destinations.destination = new XSipTrunkDestination[1];
            req.sipTrunk.destinations.destination[0] = new XSipTrunkDestination();
            req.sipTrunk.destinations.destination[0].addressIpv4 = destination;
            req.sipTrunk.destinations.destination[0].port = "5060";
            req.sipTrunk.destinations.destination[0].sortOrder = "1";
            req.sipTrunk.securityProfileName = new XFkType();
            req.sipTrunk.securityProfileName.Value = "Non Secure SIP Trunk Profile";
            req.sipTrunk.sipProfileName = new XFkType();
            req.sipTrunk.sipProfileName.Value = "Standard SIP Profile";
            req.sipTrunk.sigDigits = new XSipTrunkSigDigits();
            req.sipTrunk.sigDigits.Value = "99";

            StandardResponse res = client.addSipTrunk(req);

            return result;
        }

        public bool AddRouteGroup(string name, List<string> members)
        {
            bool result = true;

            AddRouteGroupReq req = new AddRouteGroupReq();
            req.routeGroup = new XRouteGroup();
            req.routeGroup.name = name;
            req.routeGroup.distributionAlgorithm = "Top Down";
            req.routeGroup.members = new XRouteGroupMembers();
            req.routeGroup.members.member = new XRouteGroupMember[members.Count];
            int idx = 0;
            foreach(string member in members)
            {
                req.routeGroup.members.member[idx] = new XRouteGroupMember();
                req.routeGroup.members.member[idx].deviceName = new XFkType();
                req.routeGroup.members.member[idx].deviceName.Value = members[idx];
                req.routeGroup.members.member[idx].deviceSelectionOrder = (idx+1).ToString();
                req.routeGroup.members.member[idx].port = "0";
                idx++;
            }

            StandardResponse res = client.addRouteGroup(req);
            return result;
        }

        public bool GetSIPTrunk(string name)
        {
            bool result = true;

            GetSipTrunkReq req = new GetSipTrunkReq();
            req.ItemElementName = ItemChoiceType165.name;
            req.Item = name;

            GetSipTrunkRes res = client.getSipTrunk(req);

            return result;

        }
     
        public bool Addlocation(string name)
        {
            bool result = true;

            AddLocationReq req = new AddLocationReq();
            req.location = new XLocation();
            req.location.name = name;

            StandardResponse res = client.addLocation(req);
            return result;
        }

        public bool AddPhysicalLocation(string name)
        {
            bool result = true;

            AddPhysicalLocationReq req = new AddPhysicalLocationReq();
            req.physicalLocation = new XPhysicalLocation { name = name };

            StandardResponse res = client.addPhysicalLocation(req);
            return result;
        }

        public GetPhysicalLocationRes GetPhysicalLocation(string name)
        {
            GetPhysicalLocationReq req = new GetPhysicalLocationReq();
            req.ItemElementName = ItemChoiceType67.name;
            req.Item = name;

            GetPhysicalLocationRes res = null;

            try
            {
                res = client.getPhysicalLocation(req);
            }
            catch (Exception)
            {
            }

            return res;
        }

        public GetLineRes GetLine(string number, string partition)
        {
            GetLineReq req = new GetLineReq();
            req.ItemsElementName = new ItemsChoiceType64[2];
            req.ItemsElementName[0] = ItemsChoiceType64.pattern;
            req.ItemsElementName[1] = ItemsChoiceType64.routePartitionName;
            req.Items = new Object[2];
            req.Items[0] = number;
            req.Items[1] = new XFkType { Value = partition };
            GetLineRes res = client.getLine(req);

            return res;
        }

        public bool AddLine(string number, string partition, string voicemailprofile, string css,string destinationerror, string csserror, string destinationfwdall, string cssfwdall)
        {
            bool result = true;

            AddLineReq req = new AddLineReq();
            req.line = new XLine();
            req.line.pattern = number;
            req.line.routePartitionName = new XFkType { Value = partition };
            req.line.voiceMailProfileName = new XFkType { Value = voicemailprofile };
            req.line.shareLineAppearanceCssName = new XFkType { Value = css };
            
            req.line.callForwardBusyInt = new XCallForwardBusyInt { destination = destinationerror, callingSearchSpaceName = new XFkType { Value = csserror } };
            req.line.callForwardBusy = new XCallForwardBusy { destination = destinationerror, callingSearchSpaceName = new XFkType { Value = csserror } };
            req.line.callForwardNoAnswerInt = new XCallForwardNoAnswerInt { destination = destinationerror, callingSearchSpaceName = new XFkType { Value = csserror }, duration = "32"};
            req.line.callForwardNoAnswer = new XCallForwardNoAnswer { destination = destinationerror, callingSearchSpaceName = new XFkType { Value = csserror }, duration = "32" };
            req.line.callForwardNotRegisteredInt = new XCallForwardNotRegisteredInt { destination = destinationerror, callingSearchSpaceName = new XFkType { Value = csserror } };
            req.line.callForwardNotRegistered = new XCallForwardNotRegistered { destination = destinationerror, callingSearchSpaceName = new XFkType { Value = csserror } };
            
            if (!destinationfwdall.Equals(""))
            {
                req.line.callForwardAll = new XCallForwardAll { destination = destinationfwdall, callingSearchSpaceName = new XFkType { Value = cssfwdall} };
            }


            StandardResponse res = client.addLine(req);
            return result;
        }

        public GetCtiRoutePointRes GetCtiRoutingPoint(string name)
        {
            GetCtiRoutePointReq req = new GetCtiRoutePointReq();
            req.ItemElementName = ItemChoiceType127.name;
            req.Item = name;

            GetCtiRoutePointRes res = client.getCtiRoutePoint(req);

            return res;
        }

        public bool AddCtiRoutingPoint(string name, string description,string devicepool, string css, string location, string number, string partition)
        {
            bool result = true;

            AddCtiRoutePointReq req = new AddCtiRoutePointReq();
            req.ctiRoutePoint = new XCtiRoutePoint();
            req.ctiRoutePoint.product = "CTI Route Point";
            req.ctiRoutePoint.protocol = "SCCP";
            req.ctiRoutePoint.@class = "CTI Route Point";
            req.ctiRoutePoint.name = name;
            req.ctiRoutePoint.description = description;
            req.ctiRoutePoint.devicePoolName = new XFkType { Value = devicepool };
            req.ctiRoutePoint.callingSearchSpaceName = new XFkType { Value = css };
            req.ctiRoutePoint.locationName = new XFkType { Value = location };
            req.ctiRoutePoint.lines = new XCtiRoutePointLines();
            req.ctiRoutePoint.lines.Items = new XPhoneLine[1];
            XPhoneLine ligne = new XPhoneLine();
            ligne.dirn = new XDirn { pattern = number, routePartitionName = new XFkType { Value = partition } };
            ligne.index = "1";
            req.ctiRoutePoint.lines.Items[0] = ligne;

            StandardResponse res = client.addCtiRoutePoint(req);
            return result;
        }

        public bool AddDeviceToAppUser(string appuser, string devicename)
        {
            bool result = true;

            GetAppUserReq req1 = new GetAppUserReq();
            req1.ItemElementName = ItemChoiceType105.userid;
            req1.Item = appuser;
            GetAppUserRes res1 = client.getAppUser(req1);

            UpdateAppUserReq req = new UpdateAppUserReq();
            req.ItemElementName = ItemChoiceType8.userid;
            req.Item = appuser;
            req.associatedDevices = new string[res1.@return.appUser.associatedDevices.Length + 1];
            Array.Copy(res1.@return.appUser.associatedDevices, req.associatedDevices, res1.@return.appUser.associatedDevices.Length);
            req.associatedDevices[res1.@return.appUser.associatedDevices.Length] = devicename;

            StandardResponse res = client.updateAppUser(req);
            return result;
        }

        public GetTransPatternRes GetTranslationPattern(string pattern, string partition)
        {
            GetTransPatternReq req = new GetTransPatternReq();
            req.ItemsElementName = new ItemsChoiceType54[2];
            req.ItemsElementName[0] = ItemsChoiceType54.pattern;
            req.ItemsElementName[1] = ItemsChoiceType54.routePartitionName;
            req.Items = new Object[2];
            req.Items[0] = pattern;
            req.Items[1] = new XFkType { Value = partition };
            GetTransPatternRes res = client.getTransPattern(req);

            return res;
        }

        public bool AddTranslationPattern(string pattern, string partition, string description, string css, string calledpartymask, string callingpartymask)
        {
            bool result = true;

            AddTransPatternReq req = new AddTransPatternReq();
            req.transPattern = new XTransPattern();
            req.transPattern.pattern = pattern;
            req.transPattern.routePartitionName = new XFkType { Value = partition };
            req.transPattern.description = description;
            req.transPattern.blockEnable = "false";
            req.transPattern.usage = "Translation";
            req.transPattern.patternUrgency = "true";
            req.transPattern.callingSearchSpaceName = new XFkType { Value = css };
            req.transPattern.calledPartyTransformationMask = calledpartymask;
            if (!callingpartymask.Equals(""))
            {
                req.transPattern.callingPartyTransformationMask = callingpartymask;
                req.transPattern.useCallingPartyPhoneMask = "On";
            }

            StandardResponse res = client.addTransPattern(req);
            return result;
        }

        public bool AddCallingTransformationPattern(string pattern, string partition, string description, string callingpartymask)
        {
            bool result = true;

            AddCallingPartyTransformationPatternReq req = new AddCallingPartyTransformationPatternReq();
            req.callingPartyTransformationPattern = new XCallingPartyTransformationPattern();
            req.callingPartyTransformationPattern.pattern = pattern;
            req.callingPartyTransformationPattern.routePartitionName = new XFkType { Value = partition };
            req.callingPartyTransformationPattern.description = description;
            req.callingPartyTransformationPattern.callingPartyTransformationMask = callingpartymask;

            StandardResponse res = client.addCallingPartyTransformationPattern(req);

            return result;
        }

        public bool AddCalledTransformationPattern(string pattern, string partition, string description, string calledpartymask)
        {
            bool result = true;

            AddCalledPartyTransformationPatternReq req = new AddCalledPartyTransformationPatternReq();
            req.calledPartyTransformationPattern = new XCalledPartyTransformationPattern();
            req.calledPartyTransformationPattern.pattern = pattern;
            req.calledPartyTransformationPattern.routePartitionName = new XFkType { Value = partition };
            req.calledPartyTransformationPattern.description = description;
            req.calledPartyTransformationPattern.calledPartyTransformationMask = calledpartymask;

            StandardResponse res = client.addCalledPartyTransformationPattern(req);

            return result;
        }

        // Juste pour permettre de créer une liste chez moi : manque tous les codecs
        public bool AddAudioCodecPreferenceList(string name, string description)
        {
            bool result = true;

            AddAudioCodecPreferenceListReq req = new AddAudioCodecPreferenceListReq();
            req.audioCodecPreferenceList = new XAudioCodecPreferenceList();
            req.audioCodecPreferenceList.name = name;
            req.audioCodecPreferenceList.description = description;
            req.audioCodecPreferenceList.codecsInList = new string[2];
            req.audioCodecPreferenceList.codecsInList[0] = "G.729 8k";
            req.audioCodecPreferenceList.codecsInList[1] = "OPUS (6k-510k)";

            StandardResponse res = client.addAudioCodecPreferenceList(req);

            return result;
        }
     
        public ListRegionRes GetListRegion()
        {
            ListRegionReq req = new ListRegionReq();
            req.searchCriteria = new ListRegionReqSearchCriteria();
            req.searchCriteria.name = "%";

            ListRegionRes res = client.listRegion(req);

            return res;
        }

        public bool AddRegion(string name, string codecpreflist)
        {
            bool result = true;

            AddRegionReq req = new AddRegionReq();
            req.region = new XRegion();
            req.region.name = name;

            ListRegionRes regions = GetListRegion();
            req.region.relatedRegions = new XRegionRelationship[regions.@return.Length];
            int idx = 0;
            foreach(LRegion region in regions.@return)
            {
                req.region.relatedRegions[idx] = new XRegionRelationship();
                req.region.relatedRegions[idx].regionName = new XFkType { Value = region.name };
                req.region.relatedRegions[idx].codecPreference = new XFkType { Value = codecpreflist };
                idx++;
            }

            StandardResponse res = client.addRegion(req);

            return result;
        }
    }

}
