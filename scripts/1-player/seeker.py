def standAround():
    InputState.leftKeyPressed = False
    InputState.rightKeyPressed = False


def goLeft():
    InputState.leftKeyPressed = True
    InputState.rightKeyPressed = False


def goRight():
    InputState.leftKeyPressed = False
    InputState.rightKeyPressed = True


def isOnSameRow(playerInRange):
    return playerInRange.y == 0


def directionIsWalkable(tile):
    return tile.isSolid()


def isChaseable(tile, playerInRange):
    return directionIsWalkable(tile) and isOnSameRow(playerInRange)


def isLeft(distance):
    return distance < 0


def isRight(distance):
    return distance > 0


def update():
    tiles = ProxyGameState.getTilesInRange()
    leftFloorTile = tiles[3][6]
    rightFloorTile = tiles[7][6]

    players = ProxyGameState.getPlayersInRange()
    playerInRange = players[0] if len(players) > 0 else False

    if playerInRange:
        if isLeft(playerInRange.x) and isChaseable(leftFloorTile, playerInRange):
            goLeft()
        elif isRight(playerInRange.x) and isChaseable(rightFloorTile, playerInRange):
            goRight()
    else:
        standAround()
