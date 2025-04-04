using _Game.__Scripts.System.DI.Test;
using UnityEngine;

// This file is auto-generated. Do not modify manually.

public static class GameResources
{
    public static class Prefabs
    {
        public static class UI
        {
            public static GameplayWindow GameplayWindow => Resources.Load<GameplayWindow>("Prefabs/UI/GameplayWindow");
            public static UIFacade UIFacade => Resources.Load<UIFacade>("Prefabs/UI/UIFacade");
        }
        public static CarFacade Car => Resources.Load<CarFacade>("Prefabs/Car");
        public static CameraFacade MainCamera => Resources.Load<CameraFacade>("Prefabs/MainCamera");
        public static PlayerController PC => Resources.Load<PlayerController>("Prefabs/PC");
        public static PlayerFacade Player => Resources.Load<PlayerFacade>("Prefabs/Player");
    }
}
