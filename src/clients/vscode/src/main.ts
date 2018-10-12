'use strict';

import vscode = require('vscode');
import { SessionManager } from './session';

var sessionManager: SessionManager = undefined;

export function activate(context: vscode.ExtensionContext): void {
    context.subscriptions.push(
        vscode.commands.registerCommand(
            "ShaderTools.OpenLogFolder",
            () => {
                vscode.commands.executeCommand(
                    "vscode.openFolder",
                    vscode.Uri.file(context.logPath),
                    true);
            }));

    context.subscriptions.push(sessionManager = new SessionManager(context));
    sessionManager.start();
}