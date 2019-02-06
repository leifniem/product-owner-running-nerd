
def isOnSameRow(playerInRange):
    return playerInRange.y == 0


def isLeft(playerInRange):
    return playerInRange.x < 0


def isRight(playerInRange):
    return playerInRange.x > 0


def goLeft():
    InputState.leftKeyPressed = True
    InputState.rightKeyPressed = False


def goRight():
    InputState.leftKeyPressed = False
    InputState.rightKeyPressed = True


def climbDown():
    InputState.downKeyPressed = True


def standAround():
    InputState.leftKeyPressed = False
    InputState.rightKeyPressed = False


def chasePlayer(playerInRange):
    if isLeft(playerInRange):
        goLeft()
    elif isRight(playerInRange):
        goRight()


def update():
    players = ProxyGameState.getPlayersInRange()
    playerInRange = players[0] if len(players) > 0 else False

    if playerInRange and isOnSameRow(playerInRange):
        chasePlayer(playerInRange)
    else:
        tiles = ProxyGameState.getTilesInRange()
        if tiles[5][6].isSolid() and tiles[5][5].isLadder():
            goLeft()
        elif tiles[5][6].isSolid():
            climbDown()
        else:
            standAround()
