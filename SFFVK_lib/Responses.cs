namespace SFFVK_lib
{
    //сгенерировано средствами "студии"
    internal class GroupResponse
    {
        public Response response { get; set; }
    }

    internal class Response
    {
        public int count { get; set; }
        public int[] items { get; set; }
    }


    internal class GroupsGetByIdResponse
    {
        public Response2[] response { get; set; }
    }

    internal class Response2
    {
        public int id { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
        public int is_closed { get; set; }
        public string type { get; set; }
        public int is_admin { get; set; }
        public int is_member { get; set; }
        public int is_advertiser { get; set; }
        public int members_count { get; set; }
        public string photo_50 { get; set; }
        public string photo_100 { get; set; }
        public string photo_200 { get; set; }
    }


}
