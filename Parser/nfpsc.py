from argparse import ArgumentParser
import queue

# parse arguments
parser = ArgumentParser();
parser.add_argument("-s", "--source", help = "Source folder to process")

args = parser.parse_args();

print("Source folder is : {}".format(args.source))


# find all .nps

class Context:
    def __init__(self):
        self.Parsing = False

    # parse one .nps
    def ParseNps(self, InFileName):
        # Guard for invalid call.
        if self.Parsing:
            return

        # read file lines.
        f = open(InFileName, "r")
        lines = f.readlines()
        # Close file
        f.close()

        # Init parse contexts
        self.LastState = None
        self.StateStack = queue.LifoQueue()
        self.Lines = lines
        self.Parsing = True

        # for each line
        for index in range(len(self.Lines)):
            self.CurLineIndex = index
            self.CurLine = self.Lines[index]
            self.ParseLine(self.CurLine)
        return 

    def ParseLine(self, line):

        return

    def DeterminStatement(self, code):
        if code == '-':
            return StatementClassMember()
        elif code == '--':
            return StatementClassProperty()
        elif code == '+':
            return StatementClassMethod()
        elif code == '+cmd':
            return StatementClassCommand()
        return None

class Statement:
    def __init__(self):
        return

class StatementClass(Statement):
    def __init__(self):
        return

class StatementClass_Model(StatementClass):
    def __init__(self):
        super.__init__(self)

class StatementSpClass_Player(StatementClass):
    def __init__(self):
        super.__init__(self)
    
class StatementSpClass_Game(StatementClass):
    def __init__(self):
        super.__init__(self)

class StatementSpClass_App(StatementClass):
    def __init__(self):
        super.__init__(self)

class StatementClassSub(Statement):
    def __init__(self, InBaseClass):
        Statement.__init__(self)
        self.baseClass = InBaseClass

class StatementClassMember(StatementClassSub):
    def __init__(self, InBaseClass):
        StatementClassSub.__init__(self, InBaseClass)

class StatementClassProperty(StatementClassSub):
    def __init__(self, InBaseClass):
        StatementClassSub.__init__(self, InBaseClass)
    
class StatementClassMethod(StatementClassSub):
    def __init__(self, InBaseClass):
        StatementClassSub.__init__(self, InBaseClass)


# first : parse classes, cache all class tags.

# second : analysis classes

# parse plan section
