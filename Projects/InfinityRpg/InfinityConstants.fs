﻿namespace InfinityRpg
open Nu

[<RequireQualifiedAccess>]
module Constants =

    [<RequireQualifiedAccess>]
    module Layout =

        let TileSizeI = v2iDup 48
        let TileSize = let t = TileSizeI in t.Vector2
        let TileSheetSizeC = v2iDup 4
        let TileSheetSizeI = Vector2i.Multiply (TileSheetSizeC, TileSizeI)
        let TileSheetSize = let t = TileSheetSizeI in t.Vector2
        let FieldMapSizeC = v2iDup 20
        let CharacterElevation = 1.0f
        let PropElevation = 1.0f
        let PickupElevation = 0.5f
        let EffectElevation = 2.0f

    [<RequireQualifiedAccess>]
    module Gui =

        // dissolve constants
        let DissolveDescriptor =
            { IncomingTime = 20L
              OutgoingTime = 30L
              DissolveImage = asset<Image> Assets.Gui.PackageName "Dissolve" }

        // splash constants
        let SplashData =
            { DissolveDescriptor = DissolveDescriptor
              IdlingTime = 60L
              SplashImageOpt = Some (asset<Image> Assets.Gui.PackageName "Nu") }

    [<RequireQualifiedAccess>]
    module Gameplay =

        // general data
        let CharacterWalkStep = 4
        let CharacterWalkSteps = int Layout.TileSize.X / CharacterWalkStep
        let CharacterAnimationFacingDelay = 16L
        let CharacterAnimationActingDelay = 12L // original value is 24L
        let ReactionTick = CharacterAnimationActingDelay * 2L
        let ActionTicksMax = CharacterAnimationActingDelay * 3L
        let ItemLimit = 9