isClimbingUp = True


def climbUp():
    InputState.downKeyPressed = False
    InputState.leftKeyPressed = False
    InputState.rightKeyPressed = False
    InputState.upKeyPressed = True


def climbDown():
    InputState.downKeyPressed = True
    InputState.leftKeyPressed = False
    InputState.rightKeyPressed = False
    InputState.upKeyPressed = False


def goRight():
    InputState.downKeyPressed = False
    InputState.leftKeyPressed = False
    InputState.rightKeyPressed = True
    InputState.upKeyPressed = False


def goLeft():
    InputState.downKeyPressed = False
    InputState.leftKeyPressed = True
    InputState.rightKeyPressed = False
    InputState.upKeyPressed = False


def hasReachedEndOfLadder(tiles):
    global isClimbingUp

    tileBeneathMe = tiles[5][6]
    tileDownRight = tiles[6][6]
    tileDownLeft = tiles[4][6]

    if isClimbingUp:
        beneathMeIsLadder = tileBeneathMe.isLadder()
        lastLadderIsConnectedWithSolid = tileDownRight.isSolid() or tileDownLeft.isSolid()
        return beneathMeIsLadder and lastLadderIsConnectedWithSolid
    else:
        return tileBeneathMe.isSolid()


def leaveLadder(tiles):
    global isClimbingUp

    tileDownRight = tiles[6][6]
    tileDownLeft = tiles[4][6]

    if tileDownRight.isSolid():
        goRight()
    elif tileDownLeft.isSolid():
        goLeft()
    else:
        isClimbingUp = not isClimbingUp


def getDistanceToNextLadderTile(row):
    for i in range(0, len(row)):
        if row[i].isLadder:
            return 5 - i
    return -17


def searchForNextLadder(tiles):
    global isClimbingUp

    nextLadderTileDistance = getDistanceToNextLadderTile(tiles[5])

    if nextLadderTileDistance is not -17:
        if nextLadderTileDistance < 0:
            goLeft()
        else:
            goRight()
    else:
        isClimbingUp = not isClimbingUp

# # # Problem: Er findet eine Leiter, klettert sie hoch, kann sie aber nicht mehr verlassen, weil er unbedingt nach rechts will

def update():
    global isClimbingUp

    tiles = ProxyGameState.getTilesInRange()
    tileOnMe = tiles[5][5]

    if tileOnMe.isLadder():
        climbUp() if isClimbingUp else climbDown()
    # elif hasReachedEndOfLadder(tiles):
     #   leaveLadder(tiles)
    else:
        searchForNextLadder(tiles)
