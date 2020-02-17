using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {

    /** The OrToken represents a choice between several patterns.
     * 
     *  The input shall contain only one of these patterns to be accepted by this token.
     *  The pattern 'a|b+|c' is compiled to an OrToken with three alternatives:
     *  a, b+ or c. Only one of these patterns need to appear in the input string.
     */
    public class OrToken : Token {

        public List<Token> Alternatives { get; set; }   ///< List of alternatives; only one needs to appear in the input.

        // For "backtracking", example: pattern = @"a(bc|b|x)cc", input = "abccaxc".
        public int _start;
        public Stack<Token> Stash { get; set; }

        /** Creates a new OrToken.
         */
        public OrToken( )
            : base( TokenType.Or, "|" ) {

            Alternatives = new List<Token>( );

        }


        public override bool Matches( string input, ref int cursor ) {
            _start = cursor;
            Stash = new Stack<Token>();

            for (int i = Alternatives.Count - 1; i >= 0; i--)
            {
                Stash.Push(Alternatives[i]);
            }

            //Stash.Push
            while (Stash.Any())
            {
                if (Stash.Pop().Matches(input, ref cursor))
                    return true;
            }

            cursor = _start;
            return false;
        }


        public override bool CanBacktrack( string input, ref int cursor ) {
            int cur_cursor = cursor;

            cursor = _start;
            while (Stash.Any())
            {
                if (Stash.Pop().Matches(input, ref cursor))
                    return true;
            }

            cursor = cur_cursor;
            return false;
        }


        public override void Reverse( ) {
            foreach ( Token t in Alternatives )
                t.Reverse( );
        }


        /** String representation of this token; all the alternatives are included.
         * 
         * \returns The string representation of this token.
         */
        protected override string printContent( ) {
            var sb = new StringBuilder( );
            foreach ( Token t in Alternatives )
                sb.AppendLine( t.ToString( ) );
            return sb.ToString( );
        }
    }
}
