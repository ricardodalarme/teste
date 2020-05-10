﻿using System;
using System.Windows.Forms;

partial class Editor_Classes : Form
{
    // Usado para acessar os dados da janela
    public static Editor_Classes Objects = new Editor_Classes();

    // Classe selecionada
    private Lists.Structures.Class Selected;

    public Editor_Classes()
    {
        InitializeComponent();
    }

    public static void Request()
    {
        // Lê os dados
        Globals.OpenEditor = Objects;
        Send.Request_Items();
        Send.Request_Classes();
    }

    public static void Open()
    {
        // Lista de itens
        Objects.cmbItems.Items.Clear();
        for (byte i = 1; i < Lists.Item.Length; i++) Objects.cmbItems.Items.Add(Globals.Numbering(i, Lists.Item.GetUpperBound(0), Lists.Item[i].Name));

        // Lista as classes
        Objects.List.Nodes.Clear();
        foreach (Lists.Structures.Class Class in Lists.Class.Values)
        {
            Objects.List.Nodes.Add(Class.Name);
            Objects.List.Nodes[Objects.List.Nodes.Count - 1].Tag = Class.ID;
        }
        if (Objects.List.Nodes.Count > 0) Objects.List.SelectedNode = Objects.List.Nodes[0];

        // Abre a janela
        Selection.Objects.Visible = false;
        Objects.Visible = true;
    }

    private void Groups_Visibility()
    {
        // Atualiza a visiblidade dos paineis
        grpGeneral.Visible = grpAttributes.Visible = grpSpawn.Visible = grpTexture.Visible = grpDrop.Visible = List.SelectedNode != null;
        grpItem_Add.Visible = false;
        List.Focus();
    }

    private void List_AfterSelect(object sender, TreeViewEventArgs e)
    {
        // Atualiza o valor da loja selecionada
        Selected = Lists.Class[(Guid)List.SelectedNode.Tag];

        // Limpa os dados necessários
        lstMale.Items.Clear();
        lstFemale.Items.Clear();
        lstItems.Items.Clear();
        grpItem_Add.Visible = false;

        // Lista os dados
        txtName.Text = Selected.Name;
        txtDescription.Text = Selected.Description;
        numHP.Value = Selected.Vital[(byte)Globals.Vitals.HP];
        numMP.Value = Selected.Vital[(byte)Globals.Vitals.MP];
        numStrength.Value = Selected.Attribute[(byte)Globals.Attributes.Strength];
        numResistance.Value = Selected.Attribute[(byte)Globals.Attributes.Resistance];
        numIntelligence.Value = Selected.Attribute[(byte)Globals.Attributes.Intelligence];
        numAgility.Value = Selected.Attribute[(byte)Globals.Attributes.Agility];
        numVitality.Value = Selected.Attribute[(byte)Globals.Attributes.Vitality];
        numSpawn_Map.Value = Selected.Spawn_Map;
        cmbSpawn_Direction.SelectedIndex = Selected.Spawn_Direction;
        numSpawn_X.Value = Selected.Spawn_X;
        numSpawn_Y.Value = Selected.Spawn_Y;
        for (byte i = 0; i < Selected.Tex_Male.Count; i++) lstMale.Items.Add(Selected.Tex_Male[i]);
        for (byte i = 0; i < Selected.Tex_Female.Count; i++) lstFemale.Items.Add(Selected.Tex_Female[i]);
        for (byte i = 0; i < Selected.Item.Count; i++) lstItems.Items.Add(Globals.Numbering(Selected.Item[i].Item1, Lists.Item.GetUpperBound(0), Lists.Item[Selected.Item[i].Item1].Name + " [" + Selected.Item[i].Item2 + "x]"));

        // Seleciona os primeiros itens
        if (lstMale.Items.Count > 0) lstMale.SelectedIndex = 0;
        if (lstFemale.Items.Count > 0) lstFemale.SelectedIndex = 0;
        if (lstItems.Items.Count > 0) lstItems.SelectedIndex = 0;

        // Altera a visibilidade dos grupos se necessário
        Groups_Visibility();
    }

    private void butNew_Click(object sender, EventArgs e)
    {
        // Adiciona uma loja nova
        Lists.Structures.Class Class = new Lists.Structures.Class(Guid.NewGuid());
        Class.Name = "New class";
        Lists.Class.Add(Class.ID, Class);

        // Adiciona na lista
        TreeNode Node = new TreeNode(Class.Name);
        Node.Tag = Class.ID;
        List.Nodes.Add(Node);
        List.SelectedNode = Node;
    }

    private void butRemove_Click(object sender, EventArgs e)
    {
        // Remove a loja selecionada
        if (List.SelectedNode != null)
        {
            Lists.Class.Remove(Selected.ID);
            List.SelectedNode.Remove();
            Groups_Visibility();
        }
    }

    private void butSave_Click(object sender, EventArgs e)
    {
        // Salva a dimensão da estrutura
        Send.Write_Classes();

        // Volta à janela de seleção
        Visible = false;
        Selection.Objects.Visible = true;
    }

    private void butCancel_Click(object sender, EventArgs e)
    {
        // Volta ao menu
        Visible = false;
        Selection.Objects.Visible = true;
    }

    private void txtName_TextChanged(object sender, EventArgs e)
    {
        // Atualiza a lista
        Selected.Name = txtName.Text;
        List.SelectedNode.Text = txtName.Text;
    }

    private void txtDescription_TextChanged(object sender, EventArgs e)
    {
        Selected.Description = txtDescription.Text;
    }

    private void numHP_ValueChanged(object sender, EventArgs e)
    {
        Selected.Vital[(byte)Globals.Vitals.HP] = (short)numHP.Value;
    }

    private void numMP_ValueChanged(object sender, EventArgs e)
    {
        Selected.Vital[(byte)Globals.Vitals.MP] = (short)numMP.Value;
    }

    private void numStrength_ValueChanged(object sender, EventArgs e)
    {
        Selected.Attribute[(byte)Globals.Attributes.Strength] = (short)numStrength.Value;
    }

    private void numResistance_ValueChanged(object sender, EventArgs e)
    {
        Selected.Attribute[(byte)Globals.Attributes.Resistance] = (short)numResistance.Value;
    }

    private void numIntelligence_ValueChanged(object sender, EventArgs e)
    {
        Selected.Attribute[(byte)Globals.Attributes.Intelligence] = (short)numIntelligence.Value;
    }

    private void numAgility_ValueChanged(object sender, EventArgs e)
    {
        Selected.Attribute[(byte)Globals.Attributes.Agility] = (short)numAgility.Value;
    }

    private void numVitality_ValueChanged(object sender, EventArgs e)
    {
        Selected.Attribute[(byte)Globals.Attributes.Vitality] = (short)numVitality.Value;
    }

    private void butMTexture_Click(object sender, EventArgs e)
    {
        // Adiciona a textura
        short Texture_Num = Preview.Select(Graphics.Tex_Character, 0);
        if (Texture_Num != 0)
        {
            Selected.Tex_Male.Add(Texture_Num);
            lstMale.Items.Add(Texture_Num);
        }
    }

    private void butFTexture_Click(object sender, EventArgs e)
    {
        // Adiciona a textura
        short Texture_Num = Preview.Select(Graphics.Tex_Character, 0);
        if (Texture_Num != 0)
        {
            Selected.Tex_Female.Add(Texture_Num);
            lstFemale.Items.Add(Texture_Num);
        }
    }

    private void butMDelete_Click(object sender, EventArgs e)
    {
        // Deleta a textura
        short Selected_Item = (short)lstMale.SelectedIndex;
        if (Selected_Item != -1)
        {
            lstMale.Items.RemoveAt(Selected_Item);
            Selected.Tex_Male.RemoveAt(Selected_Item);
        }
    }

    private void butFDelete_Click(object sender, EventArgs e)
    {
        // Deleta a textura
        short Selected_Item = (short)lstFemale.SelectedIndex;
        if (Selected_Item != -1)
        {
            lstFemale.Items.RemoveAt(Selected_Item);
            Selected.Tex_Female.RemoveAt(Selected_Item);
        }
    }

    private void numSpawn_Map_ValueChanged(object sender, EventArgs e)
    {
        Selected.Spawn_Map = (short)numSpawn_Map.Value;
    }

    private void cmbSpawn_Direction_SelectedIndexChanged(object sender, EventArgs e)
    {
        Selected.Spawn_Direction = (byte)cmbSpawn_Direction.SelectedIndex;
    }

    private void numSpawn_X_ValueChanged(object sender, EventArgs e)
    {
        Selected.Spawn_X = (byte)numSpawn_X.Value;
    }

    private void numSpawn_Y_ValueChanged(object sender, EventArgs e)
    {
        Selected.Spawn_Y = (byte)numSpawn_Y.Value;
    }
    private void butItem_Add_Click(object sender, EventArgs e)
    {
        // Abre a janela para adicionar o item
        cmbItems.SelectedIndex = 0;
        numItem_Amount.Value = 1;
        grpItem_Add.Visible = true;
    }

    private void butItem_Ok_Click(object sender, EventArgs e)
    {
        // Adiciona o item
        if (cmbItems.SelectedIndex >= 0)
        {
            lstItems.Items.Add(Globals.Numbering(cmbItems.SelectedIndex + 1, Lists.Item.GetUpperBound(0), Lists.Item[cmbItems.SelectedIndex + 1].Name + " [" + numItem_Amount.Value + "x]"));
            Selected.Item.Add(new Tuple<short, short>((short)(cmbItems.SelectedIndex + 1), (short)numItem_Amount.Value));
            grpItem_Add.Visible = false;
        }
    }

    private void butItem_Delete_Click(object sender, EventArgs e)
    {
        // Deleta a textura
        short Selected_Item = (short)lstItems.SelectedIndex;
        if (Selected_Item != -1)
        {
            lstItems.Items.RemoveAt(Selected_Item);
            Selected.Item.RemoveAt(Selected_Item);
        }
    }

    private void cmbItems_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Quantidade de itens
        if (cmbItems.SelectedIndex >= 0) numItem_Amount.Enabled = Lists.Item[cmbItems.SelectedIndex + 1].Stackable;
        numItem_Amount.Value = 1;
    }
}