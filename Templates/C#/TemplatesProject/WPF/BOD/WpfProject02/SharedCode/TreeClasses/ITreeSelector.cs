#region + Using Directives

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

#endregion

// user name: jeffs
// created:   10/14/2023 5:00:38 PM

namespace SharedCode.TreeClasses
{
	/*

	select node
	> if node == null return
	> if node is in selected return
	> remove from prior
	> add to selected

	deselect node
	> if node == null return
	> if node is not in selected return
	> remove from selected
	> add to prior

	add to selected
	> selected.add
	> raise node added event

	add to prior selected
	> prior.add
	> raise node added event

	remove from selected
	> selected.remove
	> raise selected node removed event

	remove from prior
	> prior.remove
	> raise prior node removed event

	*/


	public class Selection
	{
		public enum SelectedState
		{
			UNSET		= -1,
			MIXED		= 0,
			SELECTED	= 1,
			DESELECTED	= 2
		}

		public const int NULL  = 0;
		public const int TRUE  = 1;
		public const int FALSE = 2;

		public enum SelectFirstClass
		{
			UNSET				= 0,
			NODE_ONLY			= 100,
			NODE_EXTENDED		= 200,
			NODE_MULTI			= 300,
			NODE_MULTI_EX		= 400,
			LEAF_MULTI			= 500,
			TRI_STATE			= 600,
			END					= 700
		}

		public enum SelectSecondClass
		{
			UNSET = 0,
			NODES_ONLY,
			NODES_AND_LEAVES,
			LEAVES_ONLY
		}

		public enum SelectTreeAllowed
		{
			UNSET,
			YES,
			NO
		}

		// order matters - 
		public enum SelectMode
		{
			UNDEFINED				= 0,

			INDIVIDUAL				= SelectFirstClass.NODE_ONLY + 1, // node only, one at a time
			INDIVIDUALPLUS			= SelectFirstClass.NODE_ONLY + 2, // node only, one per time, + select all leaves

			EXTENDED				= SelectFirstClass.NODE_EXTENDED + 1, // select node + all child nodes
			EXTENDEDPLUS			= SelectFirstClass.NODE_EXTENDED + 2, // select node + all child nodes + select all leaves

			MULTISELECTNODE			= SelectFirstClass.NODE_MULTI + 1, // node only, many individual
			MULTISELECTNODEPLUS		= SelectFirstClass.NODE_MULTI + 2, // node only, many individual + select all leaves

			MULTISELECTNODEEX		= SelectFirstClass.NODE_MULTI_EX + 1,   // node only, many individual - ex, selecting a branch, selects the whole branch
			MULTISELECTNODEEXPLUS	= SelectFirstClass.NODE_MULTI_EX + 2, // node only, many individual - ex, selecting a branch, selects the whole branch + select leaves


			MULTISELECTLEAF			= SelectFirstClass.LEAF_MULTI + 1, // leaves only, many individual, cannot select nodes

			TRISTATE				= SelectFirstClass.TRI_STATE + 1, // tri state select none, node, node + children
			TRISTATEPLUS			= SelectFirstClass.TRI_STATE + 2, // tri state select none, node, node + children

			TRISTATEINVERTED		= SelectFirstClass.TRI_STATE + 11, // tri state select none, node, node + children
			TRISTATEINVERTEDPLUS	= SelectFirstClass.TRI_STATE + 12, // tri state select none, node, node + children

		}

		public struct SelModeData
		{
			public string Description { get; }
			public SelectFirstClass ClassFirst { get; }
			public SelectSecondClass ClassSecond { get; }
			public SelectTreeAllowed SelectTreeAllowed { get; }

			public SelModeData(string description,
				SelectFirstClass selFirstClass,
				SelectSecondClass selSecondClass,
				SelectTreeAllowed selTree
				)
			{
				Description = description;
				ClassFirst = selFirstClass;
				ClassSecond = selSecondClass;
				SelectTreeAllowed = selTree;
			}
		}

		public static Dictionary<SelectMode, SelModeData>
			SelModes = new()
			{
				{ SelectMode.UNDEFINED			     , new SelModeData("Undefined"                  , SelectFirstClass.UNSET, SelectSecondClass.UNSET, SelectTreeAllowed.UNSET) },
				{
					SelectMode.INDIVIDUAL		     ,
					new SelModeData("Individual"                 , SelectFirstClass.NODE_ONLY    , SelectSecondClass.NODES_ONLY      , SelectTreeAllowed.NO)
				},
				{
					SelectMode.INDIVIDUALPLUS         ,
					new SelModeData("IndividualPlus"             , SelectFirstClass.NODE_ONLY    , SelectSecondClass.NODES_AND_LEAVES, SelectTreeAllowed.NO)
				},
				{
					SelectMode.EXTENDED               ,
					new SelModeData("Extended"                   , SelectFirstClass.NODE_EXTENDED, SelectSecondClass.NODES_ONLY      , SelectTreeAllowed.YES )
				},
				{
					SelectMode.EXTENDEDPLUS           ,
					new SelModeData("ExtendedPlus"               , SelectFirstClass.NODE_EXTENDED, SelectSecondClass.NODES_AND_LEAVES, SelectTreeAllowed.NO)
				},
				{
					SelectMode.TRISTATE               ,
					new SelModeData("TriState"                   , SelectFirstClass.TRI_STATE    , SelectSecondClass.NODES_ONLY      , SelectTreeAllowed.YES)
				},
				{
					SelectMode.TRISTATEPLUS           ,
					new SelModeData("TriStatePlus"               , SelectFirstClass.TRI_STATE    , SelectSecondClass.NODES_AND_LEAVES, SelectTreeAllowed.NO)
				},

				{
					SelectMode.TRISTATEINVERTED               ,
					new SelModeData("TriStateInverted"           , SelectFirstClass.TRI_STATE    , SelectSecondClass.NODES_ONLY      , SelectTreeAllowed.YES)
				},
				{
					SelectMode.TRISTATEINVERTEDPLUS           ,
					new SelModeData("TriStateInvertedPlus"        , SelectFirstClass.TRI_STATE    , SelectSecondClass.NODES_AND_LEAVES, SelectTreeAllowed.NO)
				},
				{
					SelectMode.MULTISELECTNODE        ,
					new SelModeData("MultiSelectNode"            , SelectFirstClass.NODE_MULTI   , SelectSecondClass.NODES_ONLY      , SelectTreeAllowed.YES)
				},
				{
					SelectMode.MULTISELECTNODEPLUS    ,
					new SelModeData("MultiSelectNodePlus"        , SelectFirstClass.NODE_MULTI   , SelectSecondClass.NODES_AND_LEAVES, SelectTreeAllowed.YES)
				},
				{
					SelectMode.MULTISELECTNODEEX      ,
					new SelModeData("MultiSelectNodeExtended"    , SelectFirstClass.NODE_MULTI_EX, SelectSecondClass.NODES_ONLY      , SelectTreeAllowed.YES)
				},
				{
					SelectMode.MULTISELECTNODEEXPLUS  ,
					new SelModeData("MultiSelectNodeExtendedPlus", SelectFirstClass.NODE_MULTI_EX, SelectSecondClass.NODES_AND_LEAVES, SelectTreeAllowed.NO)
				},
				{
					SelectMode.MULTISELECTLEAF        ,
					new SelModeData("MutliSelectLeaf"            , SelectFirstClass.LEAF_MULTI   , SelectSecondClass.LEAVES_ONLY     , SelectTreeAllowed.NO)
				},
			};
	}
}