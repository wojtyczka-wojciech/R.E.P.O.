using BepInEx;
using HarmonyLib;
using MenuLib;
using MenuLib.MonoBehaviors;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace KickPlugin
{
    [BepInPlugin( Guid, Name, Version )]
    public class KickMod : BaseUnityPlugin
    {
        private const string Guid = "Wtyka.KickMod";
        private const string Name = "KickMod";
        private const string Version = "1.0.0";

        private REPOPopupPage showPlayerList;
        public static KickMod Instance;
        private readonly Harmony harmony = new Harmony( Guid );

        // Używamy GameObject, żeby stworzyć PhotonView

        void Awake()
        {
            if(Instance == null)
                Instance = this;

            Debug.Log( "KickMod Loaded!" );
        }

        void Update()
        {
            if(Input.GetKeyDown( KeyCode.F1 ))
            {
                if(showPlayerList != null)
                {
                    showPlayerList.ClosePage( true );
                }
                else
                    CreatePage( );
            }
        }

        private void CreatePage()
        {
            showPlayerList = MenuAPI.CreateREPOPopupPage( "Players List", shouldCachePage: false, pageDimmerVisibility: true, spacing: .5f, localPosition: Vector2.zero );

            showPlayerList.AddElementToScrollView( scrollView =>
            {
                //Create element, parent it using `scrollView`
                //Setting the Y position of an element in here is useless, it will be overwritten
                //Additionally, this delegate requires a RectTransform to be returned:
                GameObject container = new GameObject( "ButtonContainer", typeof( RectTransform ), typeof( UnityEngine.UI.VerticalLayoutGroup ) );


                container.transform.SetParent( scrollView, false );

                // Dodajemy kilka przycisków do kontenera
                foreach(Player player in PhotonNetwork.PlayerList)
                {
                    var repoButton = MenuAPI.CreateREPOButton( player.NickName + (player.IsMasterClient ? " ★" : " 🥾"), () =>
                    {
                        if(player.IsMasterClient)
                            return;

                        PhotonNetwork.RemoveRPCs( player );
                        PhotonNetwork.DestroyPlayerObjects( player );

                        PhotonNetwork.EnableCloseConnection = true;
                        PhotonNetwork.CloseConnection( player );

                        Debug.Log( $"Player kicked out: {player.NickName}" );
                    }, container.transform );

                }

                return container.GetComponent<RectTransform>( );
            } );

            showPlayerList.OpenPage( true );
        }
    }
}
