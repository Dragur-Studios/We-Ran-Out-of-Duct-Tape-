public enum GOAPKey
{
    None,

    __TARGETING__,

    Target,
    HasTarget,
    AtTarget,

    __PERCEPTION__,

    __I__SIGHT__I__,
    
    HasSeenTarget,              // BOOL 
    LastSeenTargetPosition,     // VECTOR 3
    
    __I__HEARING__I__,

    LastHeardTarget,             // BOOL
    LastHeardTargetPosition,    // VECTOR 3

    __INVESTIATION__,

    Investigating,              // BOOL
    InvestigatingCompleted,     // BOOL

    __COMBAT__,

    TargetAlive,
    TargetDamaged,
    TargetDead,
    UnderAttack,

    __TRAVERSAL__,

    Moving,                     
    MoveTarget,
    IsWandering,
    WanderDestination,

    __SURVIVAL__,

    Fed,
    HasFoodTarget,
    FoodTarget,
    KnownFood,
    AtFood,




}

