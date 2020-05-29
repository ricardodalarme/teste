﻿using System;
using System.Collections.Generic;

namespace Objects
{
    [Serializable]
    class Map : Lists.Structures.Data
    {
        // Dados
        public short Revision;
        public string Name = string.Empty;
        public byte Moral;
        public Map_Layer[] Layer = Array.Empty<Map_Layer>();
        public Map_Attribute[,] Attribute = new Map_Attribute[Game.Map_Width, Game.Map_Height];
        public byte Panorama;
        public byte Music;
        public int Color = -1;
        public Map_Weather Weather = new Map_Weather();
        public Map_Fog Fog = new Map_Fog();
        public Map_NPC[] NPC = Array.Empty<Map_NPC>();
        public Map_Light[] Light = Array.Empty<Map_Light>();
        public byte Lighting = 100;
        public Map[] Link = new Map[(byte)Game.Directions.Count];

        // Construtor
        public Map(Guid ID) : base(ID)
        {
            for (byte x = 0; x < Game.Map_Width; x++)
                for (byte y = 0; y < Game.Map_Height; y++)
                    Attribute[x, y] = new Map_Attribute();
        }

        // Verifica se as coordenas estão no limite do mapa
        public bool OutLimit(byte X, byte Y) => X >= Game.Map_Width || Y >= Game.Map_Height || X < 0 || Y < 0;

        public bool Tile_Blocked(byte X, byte Y)
        {
            // Verifica se o azulejo está bloqueado
            if (OutLimit(X, Y)) return true;
            if (Attribute[X, Y].Type == (byte)Game.Tile_Attributes.Block) return true;
            return false;
        }

        public void Create_Temporary()
        {
            TMap Temp_Map = new TMap(ID, this);
            Lists.Temp_Map.Add(ID, Temp_Map);

            // NPCs do mapa
            Temp_Map.NPC = new TNPC[NPC.Length];
            for (byte i = 1; i < Temp_Map.NPC.Length; i++)
            {
                Temp_Map.NPC[i] = new TNPC(i, Temp_Map, NPC[i].NPC);
                Temp_Map.NPC[i].Spawn();
            }

            // Itens do mapa
            Temp_Map.Spawn_Items();
        }
    }


    [Serializable]
    class Map_Attribute
    {
        public byte Type;
        public string Data_1;
        public short Data_2;
        public short Data_3;
        public short Data_4;
        public byte Zone;
        public bool[] Block = new bool[(byte)Game.Directions.Count];
    }

    [Serializable]
    class Map_Layer
    {
        public string Name;
        public byte Type;
        public Map_Tile_Data[,] Tile = new Map_Tile_Data[Game.Map_Width, Game.Map_Height];

        public Map_Layer()
        {
            for (byte x = 0; x < Game.Map_Width; x++)
                for (byte y = 0; y < Game.Map_Height; y++)
                    Tile[x, y] = new Map_Tile_Data();
        }
    }

    [Serializable]
    class Map_NPC
    {
        public NPC NPC;
        public byte Zone;
        public bool Spawn;
        public byte X;
        public byte Y;
    }

    [Serializable]
    class Map_Tile_Data
    {
        public byte X;
        public byte Y;
        public byte Tile;
        public bool Auto;
    }

    [Serializable]
    class Map_Light
    {
        public byte X;
        public byte Y;
        public byte Width;
        public byte Height;
    }

    [Serializable]
    class Map_Weather
    {
        public byte Type;
        public byte Intensity;
    }

    [Serializable]
    class Map_Fog
    {
        public byte Texture;
        public sbyte Speed_X;
        public sbyte Speed_Y;
        public byte Alpha = 255;
    }
}