﻿using System;

namespace Objects
{
    class TNPC : Character
    {
        // Dados básicos
        public byte Index;
        public NPC Data;
        public bool Alive;
        public Character Target;
        public int Spawn_Timer;
        public int Attack_Timer;

        private short Regeneration(byte Vital)
        {
            // Cálcula o máximo de vital que o NPC possui
            switch ((Game.Vitals)Vital)
            {
                case Game.Vitals.HP: return (short)(Data.Vital[Vital] * 0.05 + Data.Attribute[(byte)Game.Attributes.Vitality] * 0.3);
                case Game.Vitals.MP: return (short)(Data.Vital[Vital] * 0.05 + Data.Attribute[(byte)Game.Attributes.Intelligence] * 0.1);
            }

            return 0;
        }

        // Construtor
        public TNPC(byte Index, TMap Map, NPC Data)
        {
            this.Index = Index;
            this.Map = Map;
            this.Data = Data;
        }

        /////////////
        // Funções //
        /////////////
        public void Logic()
        {
            ////////////////
            // Surgimento //
            ////////////////
            if (!Alive)
            {
                if (Environment.TickCount > Spawn_Timer + (Data.SpawnTime * 1000)) Spawn();
                return;
            }
            else
            {
                byte TargetX = 0, TargetY = 0;
                bool[] CanMove = new bool[(byte)Game.Directions.Count];
                short Distance;
                bool Moved = false;
                bool Move = false;

                /////////////////
                // Regeneração //
                /////////////////
                if (Environment.TickCount > Loop.Timer_NPC_Regen + 5000)
                    for (byte v = 0; v < (byte)Game.Vitals.Count; v++)
                        if (Vital[v] < Data.Vital[v])
                        {
                            // Renera os vitais
                            Vital[v] += Regeneration(v);

                            // Impede que o valor passe do limite
                            if (Vital[v] > Data.Vital[v]) Vital[v] = Data.Vital[v];

                            // Envia os dados aos jogadores do mapa
                            Send.Map_NPC_Vitals(this);
                        }

                //////////////////
                // Movimentação //
                //////////////////
                // Atacar ao ver
                if (Data.Behaviour == (byte)Game.NPC_Behaviour.AttackOnSight)
                {
                    // Jogador
                    if (Target == null)
                        for (byte Player_Index = 0; Player_Index < Lists.Account.Count; Player_Index++)
                        {
                            // Verifica se o jogador está jogando e no mesmo mapa que o NPC
                            if (!Lists.Account[Player_Index].IsPlaying) continue;
                            if (Lists.Account[Player_Index].Character.Map != Map) continue;

                            // Se o jogador estiver no alcance do NPC, ir atrás dele
                            Distance = (short)Math.Sqrt(Math.Pow(X - Lists.Account[Player_Index].Character.X, 2) + Math.Pow(Y - Lists.Account[Player_Index].Character.Y, 2));
                            if (Distance <= Data.Sight)
                            {
                                Target = Lists.Account[Player_Index].Character;

                                // Mensagem
                                if (!string.IsNullOrEmpty(Data.SayMsg)) Send.Message(Lists.Account[Player_Index].Character, Data.Name + ": " + Data.SayMsg, System.Drawing.Color.White);
                                break;
                            }
                        }

                    // NPC
                    if (Data.AttackNPC && Target == null)
                        for (byte NPC_Index = 1; NPC_Index < Map.NPC.Length; NPC_Index++)
                        {
                            // Verifica se pode atacar
                            if (NPC_Index == Index) continue;
                            if (!Map.NPC[NPC_Index].Alive) continue;
                            if (Data.IsAlied(Map.NPC[NPC_Index].Data)) continue;

                            // Se o NPC estiver no alcance do NPC, ir atrás dele
                            Distance = (short)Math.Sqrt(Math.Pow(X - Map.NPC[NPC_Index].X, 2) + Math.Pow(Y - Map.NPC[NPC_Index].Y, 2));
                            if (Distance <= Data.Sight)
                            {
                                Target = Map.NPC[NPC_Index];
                                break;
                            }
                        }
                }

                // Verifica se o alvo ainda está disponível
                if (Target != null)
                    if (Target is Player && !((Player)Target).Account.IsPlaying || Target.Map != Map)
                        Target = null;
                    else if (Target is TNPC && !((TNPC)Target).Alive)
                        Target = null;

                // Evita que ele se movimente sem sentido
                if (Target != null)
                {
                    TargetX = Target.X;
                    TargetY = Target.Y;

                    // Verifica se o alvo saiu do alcance do NPC
                    if (Data.Sight < Math.Sqrt(Math.Pow(X - TargetX, 2) + Math.Pow(Y - TargetY, 2)))
                        Target = null;
                    else
                        Move = true;
                }
                else
                {
                    // Define o alvo a zona do NPC
                    if (Map.Data.NPC[Index].Zone > 0)
                        if (Map.Data.Tile[X, Y].Zone != Map.Data.NPC[Index].Zone)
                            for (byte x2 = 0; x2 <= Map.Data.Width; x2++)
                                for (byte y2 = 0; y2 <= Map.Data.Height; y2++)
                                    if (Map.Data.Tile[x2, y2].Zone == Map.Data.NPC[Index].Zone)
                                        if (!Map.Data.Tile_Blocked(x2, y2))
                                        {
                                            TargetX = x2;
                                            TargetY = y2;
                                            Move = true;
                                            break;
                                        }
                }

                // Movimenta o NPC
                if (Move)
                {
                    // Verifica como o NPC pode se mover
                    if (Vital[(byte)Game.Vitals.HP] > Data.Vital[(byte)Game.Vitals.HP] * (Data.Flee_Helth / 100.0))
                    {
                        // Para perto do alvo
                        CanMove[(byte)Game.Directions.Up] = Y > TargetY;
                        CanMove[(byte)Game.Directions.Down] = Y < TargetY;
                        CanMove[(byte)Game.Directions.Left] = X > TargetX;
                        CanMove[(byte)Game.Directions.Right] = X < TargetX;
                    }
                    else
                    {
                        // Para longe do alvo
                        CanMove[(byte)Game.Directions.Up] = Y < TargetY;
                        CanMove[(byte)Game.Directions.Down] = Y > TargetY;
                        CanMove[(byte)Game.Directions.Left] = X < TargetX;
                        CanMove[(byte)Game.Directions.Right] = X > TargetX;
                    }

                    // Aleatoriza a forma que ele vai se movimentar até o alvo
                    if (Game.Random.Next(0, 2) == 0)
                    {
                        for (byte d = 0; d < (byte)Game.Directions.Count; d++)
                            if (!Moved && CanMove[d] && this.Move((Game.Directions)d))
                                Moved = true;
                    }
                    else
                        for (short d = (byte)Game.Directions.Count - 1; d >= 0; d--)
                            if (!Moved && CanMove[d] && this.Move((Game.Directions)d))
                                Moved = true;
                }

                // Move-se aleatoriamente
                if (Data.Behaviour == (byte)Game.NPC_Behaviour.Friendly || Target == null)
                    if (Game.Random.Next(0, 3) == 0 && !Moved)
                        if (Data.Movement == Game.NPC_Movements.MoveRandomly)
                            this.Move((Game.Directions)Game.Random.Next(0, 4), 1, true);
                        else if (Data.Movement == Game.NPC_Movements.TurnRandomly)
                        {
                            Direction = (Game.Directions)Game.Random.Next(0, 4);
                            Send.Map_NPC_Direction(this);
                        }

                ////////////
                // Ataque //
                ////////////
                Attack();
            }
        }

        private void Spawn(byte X, byte Y, Game.Directions Direction = 0)
        {
            // Faz o NPC surgir no mapa
            Alive = true;
            this.X = X;
            this.Y = Y;
            this.Direction = Direction;
            for (byte i = 0; i < (byte)Game.Vitals.Count; i++) Vital[i] = Data.Vital[i];

            // Envia os dados aos jogadores
            if (Socket.Device != null) Send.Map_NPC(Map.NPC[Index]);
        }

        public void Spawn()
        {
            byte x, y;

            // Antes verifica se tem algum local de aparecimento específico
            if (Map.Data.NPC[Index].Spawn)
            {
                Spawn(Map.Data.NPC[Index].X, Map.Data.NPC[Index].Y);
                return;
            }

            // Faz com que ele apareça em um local aleatório
            for (byte i = 0; i < 50; i++) // tenta 50 vezes com que ele apareça em um local aleatório
            {
                x = (byte)Game.Random.Next(0, Map.Data.Width);
                y = (byte)Game.Random.Next(0, Map.Data.Height);

                // Verifica se está dentro da zona
                if (Map.Data.NPC[Index].Zone > 0)
                    if (Map.Data.Tile[x, y].Zone != Map.Data.NPC[Index].Zone)
                        continue;

                // Define os dados
                if (!Map.Data.Tile_Blocked( x, y))
                {
                    Spawn(x, y);
                    return;
                }
            }

            // Em último caso, tentar no primeiro lugar possível
            for (byte x2 = 0; x2 <= Map.Data.Width; x2++)
                for (byte y2 = 0; y2 <= Map.Data.Height; y2++)
                    if (!Map.Data.Tile_Blocked( x2, y2))
                    {
                        // Verifica se está dentro da zona
                        if (Map.Data.NPC[Index].Zone > 0)
                            if (Map.Data.Tile[x2, y2].Zone != Map.Data.NPC[Index].Zone)
                                continue;

                        // Define os dados
                        Spawn(x2, y2);
                        return;
                    }
        }

        private bool Move(Game.Directions Direction, byte Movement = 1, bool CheckZone = false)
        {
            short Next_X = X, Next_Y = Y;

            // Define a direção do NPC
            this.Direction = Direction;
            Send.Map_NPC_Direction(this);

            // Próximo azulejo
            Game.NextTile(Direction, ref Next_X, ref Next_Y);

            // Próximo azulejo bloqueado ou fora do limite
            if (Map.Data.OutLimit(Next_X, Next_Y)) return false;
            if (Map.Tile_Blocked(X, Y, Direction)) return false;

            // Verifica se está dentro da zona
            if (CheckZone)
                if (Map.Data.Tile[Next_X, Next_Y].Zone != Map.Data.NPC[Index].Zone)
                    return false;

            // Movimenta o NPC
            X = (byte)Next_X;
            Y = (byte)Next_Y;
            Send.Map_NPC_Movement(this, Movement);
            return true;
        }

        private void Attack()
        {
            short Next_X = X, Next_Y = Y;
            Game.NextTile(Direction, ref Next_X, ref Next_Y);

            // Apenas se necessário
            if (!Alive) return;
            if (Environment.TickCount < Attack_Timer + 750) return;
            if (Map.Tile_Blocked(X, Y, Direction, false)) return;

            // Verifica se o jogador está na frente do NPC
            if (Target is Player)
                Attack_Player(Map.HasPlayer(Next_X, Next_Y));
            // Verifica se o NPC alvo está na frente do NPC
            else if (Target is TNPC)
                Attack_NPC(Map.HasNPC(Next_X, Next_Y));
        }

        private void Attack_Player(Player Victim)
        {
            // Verifica se a vítima pode ser atacada
            if (Victim == null) return;
            if (Victim.GettingMap) return;

            // Tempo de ataque 
            Attack_Timer = Environment.TickCount;

            // Cálculo de dano
            short Attack_Damage = (short)(Data.Attribute[(byte)Game.Attributes.Strength] - Victim.Player_Defense);

            // Dano não fatal
            if (Attack_Damage > 0)
            {
                // Demonstra o ataque aos outros jogadores
                Send.Map_NPC_Attack(this, Victim.Name, Game.Target.Player);

                if (Attack_Damage < Victim.Vital[(byte)Game.Vitals.HP])
                {
                    Victim.Vital[(byte)Game.Vitals.HP] -= Attack_Damage;
                    Send.Player_Vitals(Victim);
                }
                // FATALITY
                else
                {
                    // Reseta o alvo do NPC
                    Target = null;

                    // Mata o jogador
                    Victim.Died();
                }
            }
            // Demonstra o ataque aos outros jogadores
            else
                Send.Map_NPC_Attack(this);
        }

        private void Attack_NPC(TNPC Victim)
        {
            // Verifica se a vítima pode ser atacada
            if (Victim == null) return;
            if (!Victim.Alive) return;

            // Tempo de ataque 
            Attack_Timer = Environment.TickCount;

            // Define o alvo do NPC
            Victim.Target = this;

            // Cálculo de dano
            short Attack_Damage = (short)(Data.Attribute[(byte)Game.Attributes.Strength] - Victim.Data.Attribute[(byte)Game.Attributes.Resistance]);

            // Dano não fatal
            if (Attack_Damage > 0)
            {
                // Demonstra o ataque aos outros jogadores
                Send.Map_NPC_Attack(this, Victim.Index.ToString(), Game.Target.NPC);

                if (Attack_Damage < Victim.Vital[(byte)Game.Vitals.HP])
                {
                    Victim.Vital[(byte)Game.Vitals.HP] -= Attack_Damage;
                    Send.Map_NPC_Vitals(Victim);
                }
                // FATALITY
                else
                {
                    // Reseta o alvo do NPC
                    Target = null;

                    // Mata o NPC
                    Victim.Died();
                }
            }
            // Demonstra o ataque aos outros jogadores
            else
                Send.Map_NPC_Attack(this);
        }

        public void Died()
        {
            // Solta os itens
            for (byte i = 0; i < Data.Drop.Length; i++)
                if (Data.Drop[i].Item != null)
                    if (Game.Random.Next(1, 99) <= Data.Drop[i].Chance)
                    {
                        // Dados do item
                        TMap_Items Map_Item = new TMap_Items
                        {
                            Item = Data.Drop[i].Item,
                            Amount = Data.Drop[i].Amount,
                            X = X,
                            Y = Y
                        };

                        // Solta o item
                        Map.Item.Add(Map_Item);
                    }

            // Envia os dados dos itens no chão para o mapa
            Send.Map_Items(Map);

            // Reseta os dados do NPC 
            Spawn_Timer = Environment.TickCount;
            Target = null;
            Alive = false;
            Send.Map_NPC_Died(this);
        }
    }
}