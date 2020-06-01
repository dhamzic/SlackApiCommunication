namespace DohvacanjePoruka
{

    public class OdgovorDohvacanjePoruka
    {
        public bool ok { get; set; }
        public Message[] messages { get; set; }
        public bool has_more { get; set; }
        public bool is_limited { get; set; }
        public int unread_count_display { get; set; }
        public object channel_actions_ts { get; set; }
        public int channel_actions_count { get; set; }
    }

    public class Message
    {
        public string client_msg_id { get; set; }
        public string type { get; set; }
        public string text { get; set; }
        public string user { get; set; }
        public string ts { get; set; }
        public string team { get; set; }
        public Block[] blocks { get; set; }
    }

    public class Block
    {
        public string type { get; set; }
        public string block_id { get; set; }
        public Element[] elements { get; set; }
    }

    public class Element
    {
        public string type { get; set; }
        public Element1[] elements { get; set; }
    }

    public class Element1
    {
        public string type { get; set; }
        public string text { get; set; }
        public string name { get; set; }
    }

}