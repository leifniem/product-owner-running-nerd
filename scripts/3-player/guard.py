isWalkingLeft = False

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


def isLeft(player):
    return player.x < 0


def isRight(player):
    return player.x > 0


def directionIsWalkable(tile):
    return tile.isSolid() or tile.isLadder() if tile else False


def isChaseable(tile, playerInRange):
    return directionIsWalkable(tile) and isOnSameRow(playerInRange)


def guardArea(tiles):
    global isWalkingLeft

    leftFloorTile = tiles[4][6]
    rightFloorTile = tiles[6][6]

    if not isWalkingLeft:
        if not directionIsWalkable(rightFloorTile):
            isWalkingLeft = True
    else:
        if not directionIsWalkable(leftFloorTile):
            isWalkingLeft = False

    if isWalkingLeft:
        goLeft()
    else:
        goRight()


def chaseTresspasser(tiles, playerInRange):
    leftFloorTile = tiles[4][6]
    rightFloorTile = tiles[6][6]

    if isLeft(playerInRange) and isChaseable(leftFloorTile, playerInRange):
        goLeft()
    elif isRight(playerInRange) and isChaseable(rightFloorTile, playerInRange):
        goRight()
    else:
        guardArea(tiles)


def update():
    tiles = ProxyGameState.getTilesInRange()
    players = ProxyGameState.getPlayersInRange()

    playerInRange = players[0] if len(players) > 0 else False

    if playerInRange:
        chaseTresspasser(tiles, playerInRange)
    else:
        guardArea(tiles)
