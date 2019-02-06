
def isOnSameRow(playerInRange):
    return playerInRange.y == 0


def isLeft(playerInRange):
    return playerInRange.x < 0


def isRight(playerInRange):
    return playerInRange.x > 0


def tileIsWalkable(tile):
    return tile.isSolid()


def goLeft():
    InputState.leftKeyPressed = True
    InputState.rightKeyPressed = False


def goRight():
    InputState.leftKeyPressed = False
    InputState.rightKeyPressed = True


def standAround():
    InputState.leftKeyPressed = False
    InputState.rightKeyPressed = False


def update():
    players = ProxyGameState.getPlayersInRange()
    playerInRange = players[0] if len(players) > 0 else False

    if playerInRange and isOnSameRow(playerInRange):
        tiles = ProxyGameState.getTilesInRange()
        leftFloorTile = tiles[4][6]
        rightFloorTile = tiles[7][6]

        if isLeft(playerInRange) and tileIsWalkable(leftFloorTile):
            goLeft()
        elif isRight(playerInRange) and tileIsWalkable(rightFloorTile):
            goRight()
    else:
        standAround()
