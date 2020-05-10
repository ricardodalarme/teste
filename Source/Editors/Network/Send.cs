﻿using Lidgren.Network;

partial class Send
{
    // Pacotes do cliente
    public enum Packets
    {
        Connect,
        Write_Server_Data,
        Write_Classes,
        Write_Tiles,
        Write_Maps,
        Write_NPCs,
        Write_Items,
        Write_Shops,
        Request_Server_Data,
        Request_Classes,
        Request_Tiles,
        Request_Map,
        Request_Maps,
        Request_NPCs,
        Request_Items,
        Request_Shops
    }

    private static void Packet(NetOutgoingMessage Data)
    {
        // Envia os dados ao servidor
        Socket.Device.SendMessage(Data, NetDeliveryMethod.ReliableOrdered);
    }

    public static void Connect()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Connect);
        Data.Write(Login.Objects.txtName.Text);
        Data.Write(Login.Objects.txtPassword.Text);
        Data.Write(true); // Acesso pelo editor
        Packet(Data);
    }

    public static void Request_Server_Data()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Server_Data);
        Packet(Data);
    }

    public static void Request_Classes()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Classes);
        Packet(Data);
    }

    public static void Request_Tiles()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Tiles);
        Packet(Data);
    }

    public static void Request_Map(short Map_Num)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Map);
        Data.Write(Map_Num);
        Packet(Data);
    }

    public static void Request_Maps(bool OpenEditor = false)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Maps);
        Data.Write(OpenEditor);
        Packet(Data);
    }

    public static void Request_NPCs()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_NPCs);
        Packet(Data);
    }

    public static void Request_Items()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Items);
        Packet(Data);
    }
    
    public static void Request_Shops()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Shops);
        Packet(Data);
    }

    public static void Write_Server_Data()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Server_Data);
        Data.Write(Lists.Server_Data.Game_Name);
        Data.Write(Lists.Server_Data.Welcome);
        Data.Write(Lists.Server_Data.Port);
        Data.Write(Lists.Server_Data.Max_Players);
        Data.Write(Lists.Server_Data.Max_Characters);
        Data.Write(Lists.Server_Data.Max_Party_Members);
        Data.Write(Lists.Server_Data.Max_Map_Items);
        Data.Write(Lists.Server_Data.Num_Points);
        Packet(Data);
    }

    public static void Write_Classes()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Classes);
        Data.Write((byte)Lists.Class.Count);
        foreach (Lists.Structures.Class Class in Lists.Class.Values)
        {
            // Escreve os dados
            Data.Write(Class.ID.ToString());
            Data.Write((byte)Class.Tex_Male.Count);
            Data.Write((byte)Class.Tex_Female.Count);
            Data.Write((byte)Class.Item.Count);
            Data.Write(Class.Name);
            Data.Write(Class.Description);
            for (byte i = 0; i < Class.Tex_Male.Count; i++) Data.Write(Class.Tex_Male[i]);
            for (byte i = 0; i < Class.Tex_Female.Count; i++) Data.Write(Class.Tex_Female[i]);
            Data.Write(Class.Spawn_Map);
            Data.Write(Class.Spawn_Direction);
            Data.Write(Class.Spawn_X);
            Data.Write(Class.Spawn_Y);
            for (byte i = 0; i < (byte)Globals.Vitals.Count; i++) Data.Write(Class.Vital[i]);
            for (byte i = 0; i < (byte)Globals.Attributes.Count; i++) Data.Write(Class.Attribute[i]);
            for (byte i = 0; i < Class.Item.Count; i++)
            {
                Data.Write(Class.Item[i].Item1);
                Data.Write(Class.Item[i].Item2);
            }
        }
        Packet(Data);
    }

    public static void Write_Tiles()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Tiles);
        Data.Write((byte)Lists.Tile.Length);
        for (byte i = 1; i < Lists.Tile.Length; i++)
        {
            Data.Write(Lists.Tile[i].Width);
            Data.Write(Lists.Tile[i].Height);

            for (byte x = 0; x <= Lists.Tile[i].Width; x++)
                for (byte y = 0; y <= Lists.Tile[i].Height; y++)
                {
                    Data.Write(Lists.Tile[i].Data[x, y].Attribute);

                    // Bloqueio direcional
                    for (byte d = 0; d < (byte)Globals.Directions.Count; d++)
                        Data.Write(Lists.Tile[i].Data[x, y].Block[d]);
                }
        }
        Packet(Data);
    }

    public static void Write_Maps()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Maps);
        Data.Write((short)Lists.Map.Length);
        for (short Index = 1; Index < Lists.Map.Length; Index++)
        {
            Data.Write((short)(Lists.Map[Index].Revision + 1));
            Data.Write(Lists.Map[Index].Name);
            Data.Write(Lists.Map[Index].Width);
            Data.Write(Lists.Map[Index].Height);
            Data.Write(Lists.Map[Index].Moral);
            Data.Write(Lists.Map[Index].Panorama);
            Data.Write(Lists.Map[Index].Music);
            Data.Write(Lists.Map[Index].Color);
            Data.Write(Lists.Map[Index].Weather.Type);
            Data.Write(Lists.Map[Index].Weather.Intensity);
            Data.Write(Lists.Map[Index].Fog.Texture);
            Data.Write(Lists.Map[Index].Fog.Speed_X);
            Data.Write(Lists.Map[Index].Fog.Speed_Y);
            Data.Write(Lists.Map[Index].Fog.Alpha);
            Data.Write(Lists.Map[Index].Light_Global);
            Data.Write(Lists.Map[Index].Lighting);

            // Ligações
            for (short i = 0; i < (short)Globals.Directions.Count; i++)
                Data.Write(Lists.Map[Index].Link[i]);

            // Camadas
            Data.Write((byte)(Lists.Map[Index].Layer.Count - 1));
            for (byte i = 0; i < Lists.Map[Index].Layer.Count; i++)
            {
                Data.Write(Lists.Map[Index].Layer[i].Name);
                Data.Write(Lists.Map[Index].Layer[i].Type);

                // Azulejos
                for (byte x = 0; x <= Lists.Map[Index].Width; x++)
                    for (byte y = 0; y <= Lists.Map[Index].Height; y++)
                    {
                        Data.Write(Lists.Map[Index].Layer[i].Tile[x, y].X);
                        Data.Write(Lists.Map[Index].Layer[i].Tile[x, y].Y);
                        Data.Write(Lists.Map[Index].Layer[i].Tile[x, y].Tile);
                        Data.Write(Lists.Map[Index].Layer[i].Tile[x, y].Auto);
                    }
            }


            // Dados específicos dos azulejos
            for (byte x = 0; x <= Lists.Map[Index].Width; x++)
                for (byte y = 0; y <= Lists.Map[Index].Height; y++)
                {
                    Data.Write(Lists.Map[Index].Tile[x, y].Attribute);
                    Data.Write(Lists.Map[Index].Tile[x, y].Data_1);
                    Data.Write(Lists.Map[Index].Tile[x, y].Data_2);
                    Data.Write(Lists.Map[Index].Tile[x, y].Data_3);
                    Data.Write(Lists.Map[Index].Tile[x, y].Data_4);
                    Data.Write(Lists.Map[Index].Tile[x, y].Zone);

                    // Bloqueio direcional
                    for (byte i = 0; i < (byte)Globals.Directions.Count; i++)
                        Data.Write(Lists.Map[Index].Tile[x, y].Block[i]);
                }

            // Luzes
            Data.Write((byte)Lists.Map[Index].Light.Count);
            for (byte i = 0; i < Lists.Map[Index].Light.Count; i++)
            {
                Data.Write(Lists.Map[Index].Light[i].X);
                Data.Write(Lists.Map[Index].Light[i].Y);
                Data.Write(Lists.Map[Index].Light[i].Width);
                Data.Write(Lists.Map[Index].Light[i].Height);
            }

            // NPCs
            Data.Write((byte)Lists.Map[Index].NPC.Count);
            for (byte i = 0; i < Lists.Map[Index].NPC.Count; i++)
            {
                Data.Write(Lists.Map[Index].NPC[i].Index);
                Data.Write(Lists.Map[Index].NPC[i].Zone);
                Data.Write(Lists.Map[Index].NPC[i].Spawn);
                Data.Write(Lists.Map[Index].NPC[i].X);
                Data.Write(Lists.Map[Index].NPC[i].Y);
            }
        }
        Packet(Data);
    }

    public static void Write_NPCs()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_NPCs);
        Data.Write((short)Lists.NPC.Length);
        for (short Index = 1; Index < Lists.NPC.Length; Index++)
        {
            Data.Write(Lists.NPC[Index].Name);
            Data.Write(Lists.NPC[Index].SayMsg);
            Data.Write(Lists.NPC[Index].Texture);
            Data.Write(Lists.NPC[Index].Behaviour);
            Data.Write(Lists.NPC[Index].SpawnTime);
            Data.Write(Lists.NPC[Index].Sight);
            Data.Write(Lists.NPC[Index].Experience);
            for (byte i = 0; i < (byte)Globals.Vitals.Count; i++) Data.Write(Lists.NPC[Index].Vital[i]);
            for (byte i = 0; i < (byte)Globals.Attributes.Count; i++) Data.Write(Lists.NPC[Index].Attribute[i]);
            Data.Write((byte)Lists.NPC[Index].Drop.Count);
            for (byte i = 0; i < Lists.NPC[Index].Drop.Count; i++)
            {
                Data.Write(Lists.NPC[Index].Drop[i].Item_Num);
                Data.Write(Lists.NPC[Index].Drop[i].Amount);
                Data.Write(Lists.NPC[Index].Drop[i].Chance);
            }
            Data.Write(Lists.NPC[Index].AttackNPC);
            Data.Write((byte)Lists.NPC[Index].Allie.Count);
            for (byte i = 0; i < Lists.NPC[Index].Allie.Count; i++) Data.Write(Lists.NPC[Index].Allie[i]);
            Data.Write((byte)Lists.NPC[Index].Movement);
            Data.Write(Lists.NPC[Index].Flee_Helth);
            Data.Write(Lists.GetID(Lists.NPC[Index].Shop));
        }
        Packet(Data);
    }

    public static void Write_Items()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Items);
        Data.Write((short)Lists.Item.Length);
        for (short Index = 1; Index < Lists.Item.Length; Index++)
        {
            Data.Write(Lists.Item[Index].Name); 
            Data.Write(Lists.Item[Index].Description);
            Data.Write(Lists.Item[Index].Texture);
            Data.Write(Lists.Item[Index].Type);
            Data.Write(Lists.Item[Index].Stackable);
            Data.Write(Lists.Item[Index].Bind);
            Data.Write(Lists.Item[Index].Rarity);
            Data.Write(Lists.Item[Index].Req_Level);
            Data.Write(Lists.GetID(Lists.Item[Index].Req_Class));
            Data.Write(Lists.Item[Index].Potion_Experience);
            for (byte i = 0; i < (byte)Globals.Vitals.Count; i++) Data.Write(Lists.Item[Index].Potion_Vital[i]);
            Data.Write(Lists.Item[Index].Equip_Type);
            for (byte i = 0; i < (byte)Globals.Attributes.Count; i++) Data.Write(Lists.Item[Index].Equip_Attribute[i]);
            Data.Write(Lists.Item[Index].Weapon_Damage);
        }
        Packet(Data);
    }

    public static void Write_Shops()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Shops);
        Data.Write((short)Lists.Shop.Count);
        foreach (Lists.Structures.Shop Shop in Lists.Shop.Values)
        {
            // Geral
            Data.Write(Shop.ID.ToString());
            Data.Write((byte)Shop.Sold.Count);
            Data.Write((byte)Shop.Bought.Count);
            Data.Write(Shop.Name);
            Data.Write(Shop.Currency);
            for (byte j = 0; j < Shop.Sold.Count; j++)
            {
                Data.Write(Shop.Sold[j].Item_Num);
                Data.Write(Shop.Sold[j].Amount);
                Data.Write(Shop.Sold[j].Price);
            }
            for (byte j = 0; j < Shop.Bought.Count; j++)
            {
                Data.Write(Shop.Bought[j].Item_Num);
                Data.Write(Shop.Bought[j].Amount);
                Data.Write(Shop.Bought[j].Price);
            }
        }
        
        Packet(Data);
    }
}