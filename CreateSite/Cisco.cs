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

        public string Init()
        {
            string result = "";

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                client = new AXLAPIService { Url = Url, Credentials = new NetworkCredential(Login, Password) };
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }
            
            return result;
        }

        public bool AddPartition(string name, string description)
        {
            bool result = true;

            AddRoutePartitionReq req = new AddRoutePartitionReq { routePartition = new XRoutePartition { name = name, description = description } };

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

        public string AddDevicePool(string name, string ccmg, /*string datetimegroup,*/ string region, string mrgl, string location, string physlocation, /*string devicemobilitygroup,*/ string primarylrg, string secondarylrg,string devicemobilitycss, string geolocation)
        {
            string result = "";

            AddDevicePoolReq req = new AddDevicePoolReq();
            req.devicePool = new XDevicePool();
            req.devicePool.name = name;
            req.devicePool.networkLocale = "France";
            req.devicePool.callManagerGroupName = new XFkType { Value = ccmg };
            req.devicePool.dateTimeSettingName = new XFkType { Value = "ALL_France_DTG" };
            req.devicePool.regionName = new XFkType { Value = region };
            req.devicePool.mediaResourceListName = new XFkType { Value = mrgl };
            req.devicePool.locationName = new XFkType { Value = location };
            req.devicePool.physicalLocationName = new XFkType { Value = physlocation };
            req.devicePool.deviceMobilityGroupName = new XFkType { Value = "PACIFICA_DMG" };
            req.devicePool.mobilityCssName = new XFkType { Value = devicemobilitycss };
            req.devicePool.geoLocationName = new XFkType { Value = geolocation };
            req.devicePool.localRouteGroup = new XDevicePoolLocalRouteGroup[2];
            req.devicePool.localRouteGroup[0] = new XDevicePoolLocalRouteGroup { name = "Primary Local Route Group", value = primarylrg };
            req.devicePool.localRouteGroup[1] = new XDevicePoolLocalRouteGroup { name = "Secondary Local Route Group", value = secondarylrg };

            try
            {
                StandardResponse res = client.addDevicePool(req);
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }

        public GetDevicePoolRes GetDevicePool(string name)
        {
            GetDevicePoolReq req = new GetDevicePoolReq();
            req.ItemElementName = ItemChoiceType70.name;
            req.Item = name;

            GetDevicePoolRes res = null;
            try
            {
                res = client.getDevicePool(req);
            }
            catch (Exception)
            {
            }

            return res;

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
     
        public string Addlocation(string name)
        {
            string result = "";

            AddLocationReq req = new AddLocationReq();
            req.location = new XLocation();
            req.location.name = name;

            try
            {
                StandardResponse res = client.addLocation(req);
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }

        public GetLocationRes GetLocation(string name)
        {
            GetLocationReq req = new GetLocationReq();
            req.ItemElementName = ItemChoiceType72.name;
            req.Item = name;

            GetLocationRes res = null;
            try
            {
                res = client.getLocation(req);
            }
            catch (Exception)
            {
            }

            return res;
        }

        public string AddPhysicalLocation(string name)
        {
            string result = "";

            AddPhysicalLocationReq req = new AddPhysicalLocationReq();
            req.physicalLocation = new XPhysicalLocation { name = name };

            try
            {
                StandardResponse res = client.addPhysicalLocation(req);
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

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

            GetLineRes res = null;

            try
            {
                res = client.getLine(req);
            }
            catch (Exception)
            {
            }

            return res;
        }

        public string AddLine(string number, string partition, string voicemailprofile, string css,string destinationerror, string csserror, string destinationfwdall, string cssfwdall)
        {
            string result = "";

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

            try
            {
                StandardResponse res = client.addLine(req);
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }

        public GetCtiRoutePointRes GetCtiRoutingPoint(string name)
        {
            GetCtiRoutePointReq req = new GetCtiRoutePointReq();
            req.ItemElementName = ItemChoiceType127.name;
            req.Item = name;

            GetCtiRoutePointRes res = null;

            try
            {
                res = client.getCtiRoutePoint(req);
            }
            catch (Exception)
            {
            }

            return res;
        }

        public string AddCtiRoutingPoint(string name, string description,string devicepool, string css, string location, string number, string partition)
        {
            string result = "";

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

            try
            {
                StandardResponse res = client.addCtiRoutePoint(req);
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }
            
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
     
        public ListRegionRes ListRegion()
        {
            ListRegionReq req = new ListRegionReq();
            req.searchCriteria = new ListRegionReqSearchCriteria();
            req.searchCriteria.name = "%";

            ListRegionRes res = client.listRegion(req);

            return res;
        }

        public string AddRegion(string name, string codecpreflist, string audiobitrate, string videobitrate, string immersivevideobitrate)
        {
            string result = "";

            AddRegionReq req = new AddRegionReq();
            req.region = new XRegion();
            req.region.name = name;

            ListRegionRes regions = ListRegion();
            req.region.relatedRegions = new XRegionRelationship[regions.@return.Length];
            int idx = 0;
            foreach(LRegion region in regions.@return)
            {
                req.region.relatedRegions[idx] = new XRegionRelationship();
                req.region.relatedRegions[idx].regionName = new XFkType { Value = region.name };
                req.region.relatedRegions[idx].codecPreference = new XFkType { Value = codecpreflist };
                req.region.relatedRegions[idx].bandwidth = audiobitrate;
                req.region.relatedRegions[idx].videoBandwidth = videobitrate;
                req.region.relatedRegions[idx].immersiveVideoBandwidth = immersivevideobitrate;
                idx++;
            }

            try
            {
                StandardResponse res = client.addRegion(req);
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }

        public GetRegionRes GetRegion(string name)
        {
            GetRegionReq req = new GetRegionReq();
            req.ItemElementName = ItemChoiceType65.name;
            req.Item = name;

            GetRegionRes res = null;
            try
            {
                res = client.getRegion(req);
            }
            catch (Exception)
            {
            }

            return res;
        }

        public string AddGeolocation(string name, string NAM, string PC)
        {
            string result = "";

            AddGeoLocationReq req = new AddGeoLocationReq();
            req.geoLocation = new XGeoLocation();
            req.geoLocation.name = name;
            req.geoLocation.occupantName = NAM;
            req.geoLocation.postalCode = PC;

            try
            {
                StandardResponse res = client.addGeoLocation(req);
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }
            return result;
        }

        public GetGeoLocationRes GetGeolocation(string name)
        {
            GetGeoLocationReq req = new GetGeoLocationReq();
            req.ItemElementName = ItemChoiceType110.name;
            req.Item = name;

            GetGeoLocationRes res = null;
            try
            {
                res = client.getGeoLocation(req);
            }
            catch (Exception)
            {
            }
            return res;
        }

        public string AddLineGroup(string name)
        {
            string result = "";

            AddLineGroupReq req = new AddLineGroupReq();
            req.lineGroup = new XLineGroup();
            req.lineGroup.name = name;
            req.lineGroup.rnaReversionTimeOut = "25";
            req.lineGroup.distributionAlgorithm = "Longest Idle Time";

            try
            {
                StandardResponse res = client.addLineGroup(req);
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }

        public GetLineGroupRes GetLineGroup(string name)
        {
            GetLineGroupReq req = new GetLineGroupReq();
            req.ItemElementName = ItemChoiceType90.name;
            req.Item = name;

            GetLineGroupRes res = null;
            try
            {
                res = client.getLineGroup(req);
            }
            catch (Exception)
            {
            }

            return res;
        }

        public string AddHuntList(string name, string description, string CUCMGroup, string linegroup)
        {
            string result = "";

            AddHuntListReq req = new AddHuntListReq();
            req.huntList = new XHuntList();
            req.huntList.name = name;
            req.huntList.description = name;
            req.huntList.routeListEnabled = "true";
            req.huntList.callManagerGroupName = new XFkType { Value = CUCMGroup };
            req.huntList.members = new XHuntListMembers();
            req.huntList.members.member = new XHuntListMember[1];
            req.huntList.members.member[0] = new XHuntListMember();
            req.huntList.members.member[0].lineGroupName = new XFkType { Value = linegroup };
            req.huntList.members.member[0].selectionOrder = "1";

            try
            {
                StandardResponse res = client.addHuntList(req);
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }

        public GetHuntListRes GetHuntList(string name)
        {
            GetHuntListReq req = new GetHuntListReq();
            req.ItemElementName = ItemChoiceType87.name;
            req.Item = name;

            GetHuntListRes res = null;
            try
            {
                res = client.getHuntList(req);
            }
            catch (Exception)
            {
            }

            return res;
        }

        public string AddHuntPilot(string number, string partition, string description, string huntlist)
        {
            string  result = "";

            AddHuntPilotReq req = new AddHuntPilotReq();
            req.huntPilot = new XHuntPilot();
            req.huntPilot.pattern = number;
            req.huntPilot.routePartitionName = new XFkType { Value = partition };
            req.huntPilot.description = description;
            req.huntPilot.huntListName = new XFkType { Value = huntlist };
            req.huntPilot.provideOutsideDialtone = "1";

            try
            {
                StandardResponse res = client.addHuntPilot(req);
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }

        public GetHuntPilotRes GetHuntPilot(string number, string partition)
        {
            GetHuntPilotReq req = new GetHuntPilotReq();
            req.ItemsElementName = new ItemsChoiceType60[2];
            req.ItemsElementName[0] = ItemsChoiceType60.pattern;
            req.ItemsElementName[1] = ItemsChoiceType60.routePartitionName;
            req.Items = new Object[2];
            req.Items[0] = number;
            req.Items[1] = new XFkType { Value = partition };

            GetHuntPilotRes res = null;
            try
            {
                res = client.getHuntPilot(req);
            }
            catch (Exception)
            {
            }

            return res;
        }

        public string AddTranslationPattern(string pattern, string partition, string description, string css, string calledpartymask, string callingpartymask)
        {
            string result = "";

            AddTransPatternReq req = new AddTransPatternReq();
            req.transPattern = new XTransPattern();
            req.transPattern.pattern = pattern;
            req.transPattern.routePartitionName = new XFkType { Value = partition };
            req.transPattern.description = description;
            req.transPattern.blockEnable = "false";
            req.transPattern.usage = "Translation";
            req.transPattern.patternUrgency = "true";
            //req.transPattern.provideOutsideDialtone = "1";  / A priori jamais nécessaire
            req.transPattern.callingSearchSpaceName = new XFkType { Value = css };
            req.transPattern.calledPartyTransformationMask = calledpartymask;
            if (!callingpartymask.Equals(""))
            {
                req.transPattern.callingPartyTransformationMask = callingpartymask;
                req.transPattern.useCallingPartyPhoneMask = "On";
            }

            try
            {
                StandardResponse res = client.addTransPattern(req);
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }
            return result;
        }

        public GetTransPatternRes GetTranslationPattern(string number, string partition)
        {
            GetTransPatternReq req = new GetTransPatternReq();
            req.ItemsElementName = new ItemsChoiceType54[2];
            req.ItemsElementName[0] = ItemsChoiceType54.pattern;
            req.ItemsElementName[1] = ItemsChoiceType54.routePartitionName;
            req.Items = new Object[2];
            req.Items[0] = number;
            req.Items[1] = new XFkType { Value = partition };

            GetTransPatternRes res = null;
            try
            {
                res = client.getTransPattern(req);
            }
            catch (Exception)
            {
            }

            return res;

        }

        public string AddCallingPartyTransformationPattern(string pattern, string partition, string description, string mask)
        {
            string result = "";

            AddCallingPartyTransformationPatternReq req = new AddCallingPartyTransformationPatternReq();
            req.callingPartyTransformationPattern = new XCallingPartyTransformationPattern();
            req.callingPartyTransformationPattern.pattern = pattern;
            req.callingPartyTransformationPattern.routePartitionName = new XFkType { Value = partition };
            req.callingPartyTransformationPattern.description = description;
            req.callingPartyTransformationPattern.callingPartyTransformationMask = mask;

            try
            {
                StandardResponse res = client.addCallingPartyTransformationPattern(req);
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }

        public GetCallingPartyTransformationPatternRes GetCallingPartyTransformationPattern(string number, string partition)
        {
            GetCallingPartyTransformationPatternReq req = new GetCallingPartyTransformationPatternReq();
            req.ItemsElementName = new ItemsChoiceType56[2];
            req.ItemsElementName[0] = ItemsChoiceType56.pattern;
            req.ItemsElementName[1] = ItemsChoiceType56.routePartitionName;
            req.Items = new Object[2];
            req.Items[0] = number;
            req.Items[1] = new XFkType { Value = partition };

            GetCallingPartyTransformationPatternRes res = null;
            try
            {
                res = client.getCallingPartyTransformationPattern(req);
            }
            catch (Exception)
            {
            }

            return res;
        }

        public string AddCalledPartyTransformationPattern(string pattern, string partition, string description, string mask)
        {
            string result = "";

            AddCalledPartyTransformationPatternReq req = new AddCalledPartyTransformationPatternReq();
            req.calledPartyTransformationPattern = new XCalledPartyTransformationPattern();
            req.calledPartyTransformationPattern.pattern = pattern;
            req.calledPartyTransformationPattern.routePartitionName = new XFkType { Value = partition };
            req.calledPartyTransformationPattern.description = description;
            req.calledPartyTransformationPattern.calledPartyTransformationMask = mask;

            try
            {
                StandardResponse res = client.addCalledPartyTransformationPattern(req);
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }

        public GetCalledPartyTransformationPatternRes GetCalledPartyTransformationPattern(string number, string partition)
        {
            GetCalledPartyTransformationPatternReq req = new GetCalledPartyTransformationPatternReq();
            req.ItemsElementName = new ItemsChoiceType71[2];
            req.ItemsElementName[0] = ItemsChoiceType71.pattern;
            req.ItemsElementName[1] = ItemsChoiceType71.routePartitionName;
            req.Items = new Object[2];
            req.Items[0] = number;
            req.Items[1] = new XFkType { Value = partition };

            GetCalledPartyTransformationPatternRes res = null;
            try
            {
                res = client.getCalledPartyTransformationPattern(req);
            }
            catch (Exception)
            {
            }

            return res;
        }

        public string AddDeviceMobilityInfo(string name, string subnet, string mask, string devicepool)
        {
            string result = "";

            AddDeviceMobilityReq req = new AddDeviceMobilityReq();
            req.deviceMobility = new XDeviceMobility();
            req.deviceMobility.name = name;
            req.deviceMobility.subNetDetails = new XDeviceMobilitySubNetDetails();
            req.deviceMobility.subNetDetails.Item = new XDeviceMobilitySubNetDetailsIpv4SubNetDetails();
            ((XDeviceMobilitySubNetDetailsIpv4SubNetDetails)req.deviceMobility.subNetDetails.Item).ipv4Subnet = subnet;
            ((XDeviceMobilitySubNetDetailsIpv4SubNetDetails)req.deviceMobility.subNetDetails.Item).ipv4SubNetMaskSz = mask;
            req.deviceMobility.members = new XDeviceMobilityMembers();
            req.deviceMobility.members.member = new XDevicePoolDeviceMobility[1];
            req.deviceMobility.members.member[0] = new XDevicePoolDeviceMobility();
            req.deviceMobility.members.member[0].devicePoolName = new XFkType { Value = devicepool };


            try
            {
                StandardResponse res = client.addDeviceMobility(req);
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }

        public GetDeviceMobilityRes GetDeviceMobilityInfo(string name)
        {
            GetDeviceMobilityReq req = new GetDeviceMobilityReq();
            req.ItemElementName = ItemChoiceType82.name;
            req.Item = name;

            GetDeviceMobilityRes res = null;
            try
            {
                res = client.getDeviceMobility(req);
            }
            catch (Exception)
            {
                res = null;
            }

            return res;
        }
    }
}
